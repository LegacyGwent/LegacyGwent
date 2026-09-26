#!/usr/bin/env python3
"""Small offline contract tests for source delivery and built-artifact gates."""
import importlib.util
import hashlib
import io
import json
from pathlib import Path
import tempfile
import unittest
from unittest import mock
import urllib.error
import zipfile


def module(name, filename):
    spec = importlib.util.spec_from_file_location(name, Path(__file__).with_name(filename))
    value = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(value)
    return value


delivery = module('delivery', 'premium-content.py')
artifacts = module('artifacts', 'verify-client-content.py')
publisher = module('publisher', 'publish-premium-content.py')


class SourceDelivery(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        root = Path(self.temp.name)
        self.source, self.cache, self.destination = root / 'source', root / 'cache', root / 'destination'
        self.manifest = root / 'manifest.json'
        self.source.mkdir()
        self.destination.mkdir()
        (self.source / 'card.prefab').write_text('prefab payload')
        (self.source / 'card.prefab.meta').write_text('guid: abc\n')
        catalog = json.dumps({'cards': [{'prefab': 'Assets/DynamicCards/Content/card.prefab'}]})
        (self.source / 'catalog.json').write_text(catalog)
        (self.source / 'catalog.json.meta').write_text('guid: def\nuserData: \n')
        (self.destination / 'catalog.json').write_text(catalog)
        (self.destination / 'catalog.json.meta').write_bytes(b'guid: def\r\nuserData:\r\n')
        delivery.pack(self.source, self.cache, self.manifest, 'owner/repo', 'source-v1')

    def restore(self):
        delivery.install(self.manifest, self.destination, self.cache, local_only=True)

    def test_roundtrip_preserves_assets_and_tracked_meta(self):
        meta = (self.destination / 'catalog.json.meta').read_bytes()
        self.restore()
        self.assertEqual((self.destination / 'card.prefab').read_bytes(), (self.source / 'card.prefab').read_bytes())
        self.assertEqual((self.destination / 'card.prefab.meta').read_bytes(), (self.source / 'card.prefab.meta').read_bytes())
        self.assertEqual((self.destination / 'catalog.json.meta').read_bytes(), meta)

    def test_ci_discards_only_verified_download_archive(self):
        delivery.install(self.manifest, self.destination, self.cache, local_only=True, discard_archives=True)
        self.assertEqual((self.destination / 'card.prefab').read_text(), 'prefab payload')
        self.assertFalse(list(self.cache.glob('*.zip')))

    def test_corrupt_archive_cannot_install(self):
        next(self.cache.glob('*.zip')).write_bytes(b'corrupt')
        with self.assertRaisesRegex(ValueError, 'corrupt'): self.restore()
        self.assertFalse((self.destination / 'card.prefab').exists())

    def test_catalog_drift_rejected(self):
        (self.destination / 'catalog.json').write_text('{}')
        with self.assertRaisesRegex(ValueError, 'catalog'): self.restore()

    def test_meta_mismatch_rejected_before_any_move(self):
        (self.destination / 'catalog.json.meta').write_text('guid: different\n')
        with self.assertRaisesRegex(ValueError, 'meta differs'): self.restore()
        self.assertEqual(len(list(self.destination.iterdir())), 2)

    def test_existing_user_sources_preserved(self):
        (self.destination / 'user.asset').write_text('keep')
        with self.assertRaisesRegex(ValueError, 'existing premium sources'): self.restore()
        self.assertEqual((self.destination / 'user.asset').read_text(), 'keep')

    def make_chunks(self):
        delivery.split_transport(self.manifest, self.cache, threshold=100, chunk_bytes=100)
        next(self.cache.glob('*.zip')).unlink()

    def test_chunked_roundtrip(self):
        self.make_chunks(); self.restore()
        self.assertEqual((self.destination / 'card.prefab').read_text(), 'prefab payload')

    def test_corrupt_transport_chunk_rejected(self):
        self.make_chunks(); next(self.cache.glob('*.part-*')).write_bytes(b'corrupt')
        with self.assertRaisesRegex(ValueError, 'checksum mismatch'): self.restore()
        self.assertFalse((self.destination / 'card.prefab').exists())

    def test_missing_transport_chunk_rejected(self):
        self.make_chunks(); next(self.cache.glob('*.part-*')).unlink()
        with self.assertRaisesRegex(ValueError, 'Missing transport chunk'): self.restore()
        self.assertFalse((self.destination / 'card.prefab').exists())

    def test_traversal_rejected_even_with_valid_archive_hash(self):
        archive = self.cache / 'premium-source-000.zip'
        with zipfile.ZipFile(archive, 'w') as z: z.writestr('../outside', b'x')
        manifest = json.loads(self.manifest.read_text())
        manifest['parts'] = [dict(name=archive.name, bytes=archive.stat().st_size,
                                  sha256=delivery.digest(archive), files=1, unpackedBytes=1)]
        self.manifest.write_text(json.dumps(manifest))
        with self.assertRaisesRegex(ValueError, 'Invalid archive path'): self.restore()
        self.assertFalse((self.destination.parent / 'outside').exists())


class PlayerArtifacts(unittest.TestCase):
    def fixture(self, variant='standard', target='Android'):
        root = 'assets/' if target == 'Android' else 'client_Data/StreamingAssets/'
        data = {root + 'client-content.json': json.dumps(dict(schema=1, variant=variant, target=target)).encode()}
        if target == 'Android': data['lib/arm64-v8a/libil2cpp.so'] = b'fixture'
        if variant == 'premium':
            data[root + 'DynamicCards/cards.bundle'] = b'fixture'
            data[root + 'DynamicCards/cards-000.bundle'] = b'fixture'
            data[root + 'DynamicCards/cards.index.json'] = json.dumps(dict(version=2, parts=[dict(file='cards-000.bundle')])).encode()
        return data

    def check(self, data, variant='standard', target='Android'):
        artifacts.verify_names(data, data.__getitem__, variant, target)

    def test_four_variants(self):
        for target in ('StandaloneWindows64', 'Android'):
            for variant in ('standard', 'premium'):
                self.check(self.fixture(variant, target), variant, target)

    def test_standard_rejects_accidental_premium_payload(self):
        data = self.fixture(); data['assets/DynamicCards/cards.bundle'] = b'fixture'
        with self.assertRaisesRegex(ValueError, 'contains premium'): self.check(data)

    def test_premium_missing_partition_rejected(self):
        data = self.fixture('premium'); del data['assets/DynamicCards/cards-000.bundle']
        with self.assertRaisesRegex(ValueError, 'partitions'): self.check(data, 'premium')

    def test_android_armv7_only_rejected(self):
        data = self.fixture(); del data['lib/arm64-v8a/libil2cpp.so']; data['lib/armeabi-v7a/libil2cpp.so'] = b'fixture'
        with self.assertRaisesRegex(ValueError, 'ARM64'): self.check(data)

    def test_wrong_bundle_target_rejected(self):
        with self.assertRaisesRegex(ValueError, 'platform mismatch'):
            self.check(self.fixture(target='StandaloneWindows64'))

    def test_ios_raw_marker(self):
        data = {'Data/Raw/client-content.json': b'{"schema":1,"variant":"standard","target":"iOS"}'}
        self.check(data, target='iOS')


class SourcePublication(unittest.TestCase):
    def test_transient_inventory_read_is_retried(self):
        failure = urllib.error.URLError(ConnectionResetError('fixture'))
        with mock.patch.object(publisher.urllib.request, 'urlopen', side_effect=[failure, io.BytesIO(b'{}')]) as request:
            with mock.patch.object(publisher.time, 'sleep'):
                self.assertEqual(publisher.GitHub('fixture').request('repos/owner/repo'), {})
        self.assertEqual(request.call_count, 2)

    def test_ambiguous_creation_is_not_replayed(self):
        failure = urllib.error.URLError(ConnectionResetError('fixture'))
        with mock.patch.object(publisher.urllib.request, 'urlopen', side_effect=failure) as request:
            with self.assertRaises(urllib.error.URLError):
                publisher.GitHub('fixture').request('repos/owner/repo/releases', 'POST', {})
        self.assertEqual(request.call_count, 1)

    def test_resumed_draft_retains_tag_when_published(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder); payload = b'content'; (root / 'source.zip').write_bytes(payload)
            sha = hashlib.sha256(payload).hexdigest()
            manifest = dict(repository='owner/repo', release='source-v1',
                            parts=[dict(name='source.zip', bytes=len(payload), sha256=sha)])
            (root / 'manifest.json').write_text(json.dumps(manifest))
            release = dict(id=1, draft=True, tag_name='source-v1', target_commitish='a' * 40,
                           name='Source', body='Sources', html_url='https://example.invalid/draft')

            class API:
                def __init__(self, token): pass
                def request(self, path, method='GET', data=None):
                    if '/releases/tags/' in path:
                        raise urllib.error.HTTPError(path, 404, 'Draft', {}, None)
                    if '/assets?' in path:
                        return [dict(name='source.zip', state='uploaded', size=len(payload), digest='sha256:' + sha)]
                    if method == 'PATCH':
                        # Model the observed draft behavior when identity is omitted.
                        release.update(data); release['tag_name'] = data.get('tag_name', 'untagged-temporary')
                        release['html_url'] = 'https://example.invalid/' + release['tag_name']
                        return dict(release)
                    return [dict(release)]

            args = ['publisher', '--manifest', str(root / 'manifest.json'), '--archives', str(root), '--commit', 'a' * 40]
            with mock.patch.object(publisher, 'GitHub', API), mock.patch.object(publisher, 'credential', return_value='fixture'):
                with mock.patch('sys.argv', args): publisher.main()
            self.assertFalse(release['draft'])
            self.assertEqual(release['tag_name'], 'source-v1')
            self.assertEqual(release['target_commitish'], 'a' * 40)


if __name__ == '__main__': unittest.main()
