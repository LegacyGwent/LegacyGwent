#!/usr/bin/env python3
"""Fetch official Unity 2019 Android ZIP modules; install only on explicit request."""
import argparse
import hashlib
import json
import os
from pathlib import Path, PurePosixPath
import shutil
import tempfile
import time
import urllib.error
import urllib.parse
import urllib.request
import zipfile

EXPECTED_SHA256 = {
    # Locally verified official downloads from the 2019.4.41f2 metadata URLs.
    # Metadata integrity fields are empty; these pins detect changed/replaced caches.
    'android-open-jdk-8u172-b11': '43d8964b5d97b95a87b54ed5b08b70dcb0aa4f6521a1dad4caba958e44c205b9',
    'android-sdk-ndk-tools': '7e81d69c303e47a4f0e748a6352d85cd0c8fd90a5a95ae4e076b5e5f960d3c7a',
    'android-ndk-r19': '2e0fffb02e14034f062cf18ae6738e78b239e06d8ddaa74e95097f53479d65d9',
    'android-sdk-build-tools-30.0.2': '426d8f4762dc464c1d6eb18494b249541be5ecd495f8e9cc4678e90a2bc80c11',
    'android-sdk-platform-tools-28.0.1': 'db78f726d5dc653706dcd15a462ab1b946c643f598df76906c4c1858411c54df',
    'android-sdk-platforms-29': '951da8bf175254da74626824f919bd28def64f8828f29dd3b124a535cf4049d8',
    'android-sdk-platforms-30': 'f3f5b75744dbf6ee6ed3e8174a71e513bfee502d0bc3463ea97e517bff68d84e',
}


def modules(metadata):
    releases = json.loads(metadata.read_text(encoding='utf-8'))['results']
    release = next(x for x in releases if x['version'] == '2019.4.41f2')
    windows = next(x for x in release['downloads'] if x['platform'] == 'WINDOWS' and x['architecture'] == 'X86_64')
    android = next(x for x in windows['modules'] if x['id'] == 'android')
    pending = list(android['subModules'])
    while pending:
        item = pending.pop(0)
        if item['type'] == 'ZIP':
            yield item
        pending[0:0] = item.get('subModules', [])


def verified_zip(path, expected_hash):
    if not path.is_file() or path.stat().st_size == 0:
        return False
    try:
        sha = hashlib.sha256()
        with path.open('rb') as source:
            for block in iter(lambda: source.read(4 * 1024 * 1024), b''):
                sha.update(block)
        if sha.hexdigest() != expected_hash:
            return False
        with zipfile.ZipFile(path) as archive:
            return archive.testzip() is None
    except (OSError, zipfile.BadZipFile):
        return False


def download(item, cache, opener, offline=False):
    filename = Path(urllib.parse.urlparse(item['url']).path).name
    if not filename.lower().endswith('.zip') or filename != PurePosixPath(filename).name:
        raise ValueError('Unsafe Unity module archive name')
    target = cache / filename
    expected_hash = EXPECTED_SHA256[item['id']]
    if verified_zip(target, expected_hash):
        print('CACHED', item['id'], target.stat().st_size, flush=True)
        return target
    if offline:
        raise FileNotFoundError('Missing or invalid cached ZIP: ' + str(target))
    partial = target.with_name(target.name + '.partial')
    for attempt in range(1, 7):
        offset = partial.stat().st_size if partial.exists() else 0
        request = urllib.request.Request(item['url'], headers={'Range': f'bytes={offset}-'} if offset else {})
        try:
            with opener.open(request, timeout=120) as response:
                if offset:
                    content_range = response.headers.get('Content-Range', '')
                    if response.status == 206 and content_range.startswith(f'bytes {offset}-'):
                        mode = 'ab'
                    elif response.status == 200:
                        mode = 'wb'
                    else:
                        raise ValueError('Unexpected partial download response')
                else:
                    mode = 'wb'
                with partial.open(mode) as output:
                    shutil.copyfileobj(response, output, 4 * 1024 * 1024)
            if not verified_zip(partial, expected_hash):
                partial.unlink()
                raise ValueError('Downloaded ZIP failed archive integrity check')
            partial.replace(target)
            print('DOWNLOADED', item['id'], target.stat().st_size, flush=True)
            return target
        except (OSError, urllib.error.URLError, ValueError, zipfile.BadZipFile) as error:
            if attempt == 6:
                raise RuntimeError(f'Download failed for {item["id"]} after six attempts') from error
            print('RETRY', item['id'], attempt, type(error).__name__, flush=True)
            time.sleep(min(15, attempt * 2))


def install(item, archive_path, editor):
    android_root = (editor / 'Editor/Data/PlaybackEngines/AndroidPlayer').resolve()
    destination = Path(item['destination'].replace('{UNITY_PATH}', str(editor))).resolve()
    if destination != android_root and android_root not in destination.parents:
        raise ValueError('Module destination escapes AndroidPlayer')
    rename = item.get('extractedPathRename')
    if rename:
        renamed_from = Path(rename['from'].replace('{UNITY_PATH}', str(editor))).resolve()
        renamed_to = Path(rename['to'].replace('{UNITY_PATH}', str(editor))).resolve()
        if android_root not in renamed_from.parents or android_root not in renamed_to.parents:
            raise ValueError('Module rename escapes AndroidPlayer')
    else:
        renamed_from = renamed_to = None
    if not android_root.is_dir():
        raise FileNotFoundError('Install Android Build Support EXE before ZIP modules: ' + str(android_root))
    # Stage on the same volume; only verified archive paths are copied into the editor.
    with tempfile.TemporaryDirectory(prefix='unity-android-', dir=android_root.parent) as temporary:
        staging = Path(temporary)
        with zipfile.ZipFile(archive_path) as archive:
            seen = set()
            for entry in archive.infolist():
                name = PurePosixPath(entry.filename)
                if name.is_absolute() or '..' in name.parts or '\\' in entry.filename or ':' in entry.filename:
                    raise ValueError('Unsafe ZIP member: ' + entry.filename)
                if entry.is_dir():
                    continue
                if str(name).casefold() in seen:
                    raise ValueError('Duplicate ZIP member: ' + entry.filename)
                seen.add(str(name).casefold())
                if entry.external_attr >> 16 & 0o170000 == 0o120000:
                    raise ValueError('ZIP symlink refused: ' + entry.filename)
                extracted = staging.joinpath(*name.parts)
                extracted.parent.mkdir(parents=True, exist_ok=True)
                with archive.open(entry) as source, extracted.open('wb') as output:
                    shutil.copyfileobj(source, output, 4 * 1024 * 1024)
        source_root = staging
        target_root = destination
        if renamed_from:
            relative = renamed_from.relative_to(destination)
            source_root = staging / relative
            target_root = renamed_to
            if not source_root.is_dir():
                raise FileNotFoundError('Expected extracted module root missing: ' + str(relative))
        if source_root == staging:
            target_root.mkdir(parents=True, exist_ok=True)
            for child in source_root.iterdir():
                target = target_root / child.name
                if target.exists():
                    raise FileExistsError('Refusing to overwrite module path: ' + str(target))
                shutil.move(str(child), str(target))
        else:
            if target_root.exists():
                raise FileExistsError('Refusing to overwrite module path: ' + str(target_root))
            target_root.parent.mkdir(parents=True, exist_ok=True)
            shutil.move(str(source_root), str(target_root))
    print('INSTALLED', item['id'], target_root, flush=True)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--metadata', type=Path, required=True)
    parser.add_argument('--cache', type=Path, required=True)
    parser.add_argument('--editor', type=Path, required=True)
    parser.add_argument('--proxy', default='http://127.0.0.1:7890')
    parser.add_argument('--install', action='store_true', help='Explicitly extract after Android support EXE installation')
    parser.add_argument('--offline', action='store_true', help='Use only previously verified cached ZIPs')
    args = parser.parse_args()
    args.cache.mkdir(parents=True, exist_ok=True)
    if args.install and not (args.editor / 'Editor/Data/PlaybackEngines/AndroidPlayer').is_dir():
        parser.error('Install Android Build Support EXE first')
    opener = urllib.request.build_opener(urllib.request.ProxyHandler({'http': args.proxy, 'https': args.proxy}))
    for item in modules(args.metadata):
        archive = download(item, args.cache, opener, args.offline)
        if args.install:
            install(item, archive, args.editor)
    print('UNITY_ANDROID_ZIP_MODULES_READY', 'installed' if args.install else 'downloaded', flush=True)


if __name__ == '__main__':
    main()
