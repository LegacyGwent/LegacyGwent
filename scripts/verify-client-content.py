#!/usr/bin/env python3
"""Verify Addressables, standard/premium membership and Android ARM64 in a built artifact."""
import argparse
import json
from pathlib import Path
import zipfile


RUNTIME_PATH = '{UnityEngine.AddressableAssets.Addressables.RuntimePath}'


def verify_addressables(names, read, size, streaming_root, target):
    # Addressables 1.18.19 embeds runtime settings and a JSON catalog beside
    # the marker. Validate catalog references, not arbitrary bundle membership.
    root = streaming_root + 'aa/'

    def require(path):
        if path not in names or size(path) == 0:
            raise ValueError('Missing or empty Addressables content: ' + path)

    def local_path(internal_id):
        value = internal_id.replace('\\', '/')
        if not value.startswith(RUNTIME_PATH + '/'):
            return None  # Remote catalogs/bundles are not embedded payload.
        relative = value[len(RUNTIME_PATH) + 1:]
        if any(part in ('', '.', '..') for part in relative.split('/')):
            raise ValueError('Unsafe Addressables runtime path: ' + internal_id)
        return root + relative

    settings_path = root + 'settings.json'
    require(settings_path)
    settings = json.loads(read(settings_path))
    if settings.get('m_buildTarget') != target:
        raise ValueError('Addressables platform mismatch')
    catalogs = [local_path(location['m_InternalId'])
                for location in settings.get('m_CatalogLocations', [])]
    catalogs = [path for path in catalogs if path is not None]
    if not catalogs:
        raise ValueError('Missing Addressables local catalog reference')
    bundles = set()
    for path in catalogs:
        require(path)
        catalog = json.loads(read(path))
        prefixes = catalog.get('m_InternalIdPrefixes') or []
        for value in catalog.get('m_InternalIds', []):
            # Match ContentCatalogData.ExpandInternalId's optional prefix table.
            index, separator, suffix = value.rpartition('#')
            if prefixes and separator and index.lstrip('-').isdigit():
                if not 0 <= int(index) < len(prefixes):
                    raise ValueError('Invalid Addressables internal ID prefix')
                value = prefixes[int(index)] + suffix
            path = local_path(value)
            if path is not None and path.endswith('.bundle'):
                bundles.add(path)
    if not bundles:
        raise ValueError('Missing Addressables local bundle references')
    for path in sorted(bundles):
        require(path)


def verify_names(names, read, variant, target, size=None):
    names = [n.replace('\\', '/') for n in names]
    markers = [n for n in names if n.endswith('/StreamingAssets/client-content.json') or n.endswith('/Raw/client-content.json') or n == 'assets/client-content.json']
    if len(markers) != 1: raise ValueError('Expected one embedded client content manifest')
    marker = json.loads(read(markers[0]))
    if marker.get('schema') != 1 or marker.get('variant') != variant or marker.get('target') != target:
        raise ValueError('Embedded variant or platform mismatch')
    streaming_root = markers[0].rsplit('/', 1)[0] + '/'
    verify_addressables(set(names), read, size or (lambda n: len(read(n))), streaming_root, target)
    root = streaming_root + 'DynamicCards/'
    payload = [n for n in names if n.startswith(root) and (n.endswith('.bundle') or n.endswith('cards.index.json'))]
    if variant == 'standard':
        if payload: raise ValueError('Standard player contains premium animation payload')
    else:
        index_path = root + 'cards.index.json'
        if index_path not in names or root + 'cards.bundle' not in names: raise ValueError('Premium player missing catalog/index')
        index = json.loads(read(index_path))
        if index.get('version') != 2 or not index.get('parts'): raise ValueError('Invalid premium bundle index')
        expected = {root + 'cards.bundle', index_path}
        for part in index['parts']:
            name = part['file']
            if '/' in name or '\\' in name or not name.startswith('cards-') or not name.endswith('.bundle'):
                raise ValueError('Unsafe bundle name')
            expected.add(root + name)
        if set(payload) != expected: raise ValueError('Missing or unexpected premium partitions')
    if target == 'Android' and not any(n.startswith('lib/arm64-v8a/') and n.endswith('.so') for n in names):
        raise ValueError('Android player is missing ARM64 native libraries')


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('artifact', type=Path)
    p.add_argument('--variant', choices=['standard', 'premium'], required=True)
    p.add_argument('--target', required=True)
    a = p.parse_args()
    if a.artifact.is_dir():
        files = {f.relative_to(a.artifact).as_posix(): f for f in a.artifact.rglob('*') if f.is_file()}
        verify_names(files, lambda n: files[n].read_bytes(), a.variant, a.target,
                     lambda n: files[n].stat().st_size)
    else:
        with zipfile.ZipFile(a.artifact) as z:
            verify_names(z.namelist(), z.read, a.variant, a.target,
                         lambda n: z.getinfo(n).file_size)
    print('CLIENT_CONTENT_VERIFIED', a.target, a.variant)


if __name__ == '__main__': main()
