#!/usr/bin/env python3
"""Versioned premium source delivery. No dependencies beyond Python 3.9+."""
import argparse
import hashlib
import json
import os
from pathlib import Path, PurePosixPath
import shutil
import stat
import tempfile
import urllib.request
import zipfile

ROOT = Path(__file__).resolve().parents[1]
CONTENT = ROOT / 'src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/DynamicCards/Content'
MANIFEST = ROOT / 'build-config/premium-content.json'


def digest(path):
    h = hashlib.sha256()
    with Path(path).open('rb') as f:
        for block in iter(lambda: f.read(8 * 1024 * 1024), b''):
            h.update(block)
    return h.hexdigest()


def safe_name(name):
    p = PurePosixPath(name)
    if not name or '\\' in name or ':' in name or p.is_absolute() or '..' in p.parts or str(p) != name:
        raise ValueError('Invalid archive path: ' + name)
    return p


def pack(source, output, manifest, repository, tag, catalog_override=None, part_bytes=1024**3):
    source, output, manifest = Path(source).resolve(), Path(output).resolve(), Path(manifest)
    if output == source or source in output.parents:
        raise ValueError('Archive output must be outside the source tree')
    catalog = Path(catalog_override) if catalog_override else source / 'catalog.json'
    cards = json.loads(catalog.read_text(encoding='utf-8-sig'))['cards']
    prefix = 'Assets/DynamicCards/Content/'
    for card in cards:
        for field in ('prefab', 'audio'):
            value = card.get(field)
            if value and (not value.startswith(prefix) or not (source / safe_name(value[len(prefix):])).is_file()):
                raise ValueError('Missing catalog source: ' + value)
    files = sorted(p for p in source.rglob('*') if p.is_file())
    if any(p.is_symlink() for p in files):
        raise ValueError('Source symlinks are not supported')
    output.mkdir(parents=True, exist_ok=True)
    groups, group, size = [], [], 0
    for p in files:
        length = p.stat().st_size
        if length > part_bytes:
            raise ValueError('Source file exceeds archive limit: ' + str(p))
        if group and size + length > part_bytes:
            groups.append(group); group, size = [], 0
        group.append(p); size += length
    if group:
        groups.append(group)
    parts = []
    for i, group in enumerate(groups):
        name = 'premium-source-%03d.zip' % i
        archive = output / name
        # ZIP_STORED is too large for text animation sources. Deflate compresses
        # each member independently and permits streaming extraction on runners.
        with zipfile.ZipFile(archive, 'w', zipfile.ZIP_DEFLATED, compresslevel=3, allowZip64=True) as z:
            for p in group:
                info = zipfile.ZipInfo(p.relative_to(source).as_posix(), (2020, 1, 1, 0, 0, 0))
                info.compress_type = zipfile.ZIP_DEFLATED
                info.external_attr = 0o100644 << 16
                actual = catalog if p == source / 'catalog.json' else p
                with actual.open('rb') as src, z.open(info, 'w', force_zip64=True) as dst:
                    shutil.copyfileobj(src, dst, 1024 * 1024)
        item = dict(name=name, bytes=archive.stat().st_size, sha256=digest(archive),
                    unpackedBytes=sum((catalog if p == source / 'catalog.json' else p).stat().st_size for p in group), files=len(group))
        parts.append(item)
        print('PACKED', name, item['bytes'], flush=True)
    document = dict(schema=1, repository=repository, release=tag, catalogSha256=digest(catalog),
                    cards=len(cards), parts=parts)
    manifest.parent.mkdir(parents=True, exist_ok=True)
    manifest.write_text(json.dumps(document, indent=2) + '\n', encoding='utf-8')
    print('MANIFEST', manifest, 'ARCHIVE_BYTES', sum(x['bytes'] for x in parts), flush=True)


def install(manifest, destination, cache, local_only=False):
    manifest, destination, cache = Path(manifest), Path(destination).resolve(), Path(cache).resolve()
    m = json.loads(manifest.read_text(encoding='utf-8'))
    if m.get('schema') != 1 or not m.get('parts'):
        raise ValueError('Invalid content manifest')
    # Never overwrite the catalog from the checked-out commit with another version.
    catalog = destination / 'catalog.json'
    if catalog.exists() and digest(catalog) != m['catalogSha256']:
        raise ValueError('Source manifest does not match the checked-out catalog')
    extras = [p for p in destination.iterdir() if p.name not in ('catalog.json', 'catalog.json.meta')] if destination.exists() else []
    if extras:
        raise ValueError('Destination has existing premium sources; use a clean checkout')
    destination.parent.mkdir(parents=True, exist_ok=True)
    cache.mkdir(parents=True, exist_ok=True)
    if shutil.disk_usage(destination.parent).free < sum(x['unpackedBytes'] for x in m['parts']) + 1024**3:
        raise ValueError('Insufficient disk space to extract premium sources')
    with tempfile.TemporaryDirectory(prefix='.premium-stage-', dir=destination.parent) as temp:
        stage = Path(temp)
        seen = set()
        for part in m['parts']:
            name = part['name']
            if safe_name(name).name != name:
                raise ValueError('Archive names must be basenames')
            archive = cache / name
            if not archive.is_file() or archive.stat().st_size != part['bytes'] or digest(archive) != part['sha256']:
                if local_only:
                    raise ValueError('Missing or corrupt content archive: ' + name)
                repo, tag = m['repository'], m['release']
                if len(repo.split('/')) != 2 or any(safe_name(x).name != x for x in repo.split('/') + [tag]):
                    raise ValueError('Invalid release location')
                url = 'https://github.com/%s/releases/download/%s/%s' % (repo, tag, name)
                partial = archive.with_suffix('.partial')
                try:
                    with urllib.request.urlopen(url, timeout=120) as src, partial.open('wb') as dst:
                        shutil.copyfileobj(src, dst, 1024 * 1024)
                    if partial.stat().st_size != part['bytes'] or digest(partial) != part['sha256']:
                        raise ValueError('Downloaded archive checksum mismatch: ' + name)
                    partial.replace(archive)
                finally:
                    if partial.exists(): partial.unlink()
            with zipfile.ZipFile(archive) as z:
                infos = z.infolist()
                if len(infos) != part['files'] or sum(x.file_size for x in infos) != part['unpackedBytes']:
                    raise ValueError('Archive inventory mismatch')
                for info in infos:
                    name = str(safe_name(info.filename))
                    if name.casefold() in seen or info.is_dir() or stat.S_ISLNK(info.external_attr >> 16):
                        raise ValueError('Duplicate or unsupported archive entry: ' + name)
                    seen.add(name.casefold())
                    target = stage / name
                    target.parent.mkdir(parents=True, exist_ok=True)
                    with z.open(info) as src, target.open('wb') as dst:
                        shutil.copyfileobj(src, dst, 1024 * 1024)
            print('VERIFIED', part['name'], flush=True)
        if digest(stage / 'catalog.json') != m['catalogSha256']:
            raise ValueError('Extracted catalog mismatch')
        # Validate BOTH tracked files before moving anything. Unity metas may
        # differ only by Git line endings or harmless trailing YAML whitespace.
        for name in ('catalog.json', 'catalog.json.meta'):
            current = destination / name
            if not current.exists(): continue
            expected, actual = (stage / name).read_bytes(), current.read_bytes()
            if name.endswith('.meta'):
                expected = b'\n'.join(line.rstrip() for line in expected.splitlines())
                actual = b'\n'.join(line.rstrip() for line in actual.splitlines())
            if expected != actual:
                raise ValueError('Tracked catalog or meta differs from release: ' + name)
        destination.mkdir(parents=True, exist_ok=True)
        for p in stage.iterdir():
            if (destination / p.name).exists(): continue
            shutil.move(str(p), str(destination / p.name))
    print('CONTENT_READY', m['release'], m['cards'], flush=True)


def main():
    p = argparse.ArgumentParser(description=__doc__)
    commands = p.add_subparsers(dest='command', required=True)
    a = commands.add_parser('pack')
    a.add_argument('--source', type=Path, default=CONTENT)
    a.add_argument('--output', type=Path, required=True)
    a.add_argument('--manifest', type=Path, default=MANIFEST)
    a.add_argument('--catalog-override', type=Path)
    a.add_argument('--repository', required=True)
    a.add_argument('--tag', required=True)
    a = commands.add_parser('restore')
    a.add_argument('--manifest', type=Path, default=MANIFEST)
    a.add_argument('--destination', type=Path, default=CONTENT)
    a.add_argument('--cache', type=Path, default=ROOT / '.premium-downloads')
    a.add_argument('--local-only', action='store_true')
    args = vars(p.parse_args()); command = args.pop('command')
    if command == 'pack': pack(**args)
    else: install(**args)


if __name__ == '__main__':
    main()
