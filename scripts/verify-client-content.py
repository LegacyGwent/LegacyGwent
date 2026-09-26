#!/usr/bin/env python3
"""Verify standard/premium membership and Android ARM64 in an actual built artifact."""
import argparse
import json
from pathlib import Path
import zipfile


def verify_names(names, read, variant, target):
    names = [n.replace('\\', '/') for n in names]
    markers = [n for n in names if n.endswith('/StreamingAssets/client-content.json') or n.endswith('/Raw/client-content.json') or n == 'assets/client-content.json']
    if len(markers) != 1: raise ValueError('Expected one embedded client content manifest')
    marker = json.loads(read(markers[0]))
    if marker.get('schema') != 1 or marker.get('variant') != variant or marker.get('target') != target:
        raise ValueError('Embedded variant or platform mismatch')
    root = markers[0].rsplit('/', 1)[0] + '/DynamicCards/'
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
        verify_names(files, lambda n: files[n].read_bytes(), a.variant, a.target)
    else:
        with zipfile.ZipFile(a.artifact) as z: verify_names(z.namelist(), z.read, a.variant, a.target)
    print('CLIENT_CONTENT_VERIFIED', a.target, a.variant)


if __name__ == '__main__': main()
