#!/usr/bin/env python3
"""Upload pinned source archives to the manifest's repository; retry-safe, no overwrites."""
import argparse
from concurrent.futures import ThreadPoolExecutor
import hashlib
import http.client
import json
import os
from pathlib import Path
import subprocess
import socket
import time
import urllib.error
import urllib.parse
import urllib.request


def credential(account):
    token = os.environ.get('GH_TOKEN') or os.environ.get('GITHUB_TOKEN')
    if token: return token
    result = subprocess.run(['git', 'credential', 'fill'],
                            input='protocol=https\nhost=github.com\nusername=' + account + '\n\n',
                            text=True, capture_output=True, check=True)
    return dict(line.split('=', 1) for line in result.stdout.splitlines() if '=' in line)['password']


class GitHub:
    def __init__(self, token): self.token = token

    def request(self, path, method='GET', data=None):
        body = None if data is None else json.dumps(data).encode()
        req = urllib.request.Request('https://api.github.com/' + path, data=body, method=method,
              headers={'Authorization': 'Bearer ' + self.token, 'User-Agent': 'LegacyGwent-content-delivery',
                       'Accept': 'application/vnd.github+json', 'Content-Type': 'application/json'})
        with urllib.request.urlopen(req, timeout=60) as r:
            raw = r.read()
            return json.loads(raw) if raw else None

    def upload(self, url, path):
        address = urllib.parse.urlsplit(url.split('{')[0] + '?name=' + urllib.parse.quote(path.name))
        proxy = urllib.request.getproxies().get('https')
        if proxy:
            proxy = urllib.parse.urlsplit(proxy if '://' in proxy else 'http://' + proxy)
            if proxy.scheme != 'http' or proxy.username:
                raise ValueError('Only an unauthenticated HTTP CONNECT proxy is supported')
            conn = http.client.HTTPSConnection(proxy.hostname, proxy.port or 80, timeout=120)
            conn.set_tunnel(address.hostname, 443)
        else:
            conn = http.client.HTTPSConnection(address.hostname, timeout=120)
        try:
            conn.putrequest('POST', address.path + '?' + address.query)
            for key, value in {'Authorization': 'Bearer ' + self.token,
                 'User-Agent': 'LegacyGwent-content-delivery', 'Content-Type': 'application/octet-stream',
                 'Content-Length': str(path.stat().st_size)}.items(): conn.putheader(key, value)
            conn.endheaders()
            conn.sock.setsockopt(socket.SOL_SOCKET, socket.SO_SNDBUF, 8 * 1024 * 1024)
            with path.open('rb') as f:
                sent = 0
                for block in iter(lambda: f.read(1024 * 1024), b''):
                    conn.send(block); sent += len(block)
                    if sent % (64 * 1024 * 1024) == 0:
                        print('SENDING', path.name, sent, '/', path.stat().st_size, flush=True)
            response = conn.getresponse(); raw = response.read()
            if response.status != 201: raise http.client.HTTPException('GitHub upload HTTP ' + str(response.status))
            return json.loads(raw)
        finally: conn.close()


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--manifest', type=Path, default=Path(__file__).resolve().parents[1] / 'build-config/premium-content.json')
    p.add_argument('--archives', type=Path, required=True)
    p.add_argument('--commit', required=True)
    p.add_argument('--workers', type=int, choices=range(1, 17), default=8)
    a = p.parse_args(); m = json.loads(a.manifest.read_text())
    repo = m['repository']; api = GitHub(credential(repo.split('/')[0])); prefix = 'repos/' + repo
    try: release = api.request(prefix + '/releases/tags/' + m['release'])
    except urllib.error.HTTPError as e:
        if e.code != 404: raise
        # GitHub's tag endpoint can hide drafts. Reuse the existing draft on retry.
        drafts = api.request(prefix + '/releases?per_page=100')
        release = next((r for r in drafts if r['tag_name'] == m['release']), None)
        if release is None: release = api.request(prefix + '/releases', 'POST', dict(tag_name=m['release'], target_commitish=a.commit,
                  name='Premium source assets ' + m['release'], draft=True, prerelease=True,
                  body='Build-time source assets for LegacyGwent. Restore using scripts/premium-content.py and the SHA-256 manifest in build-config/premium-content.json. Not a playable client.'))
    parts = [piece for part in m['parts'] for piece in part.get('chunks', [part])]
    initial_assets = api.request(prefix + '/releases/%s/assets?per_page=100' % release['id'])
    if len(parts) > 100: raise ValueError('Publisher currently supports up to 100 transport files')
    def upload_part(part):
        path = a.archives / part['name']
        h = hashlib.sha256()
        with path.open('rb') as f:
            for block in iter(lambda: f.read(8 * 1024 * 1024), b''): h.update(block)
        actual = h.hexdigest()
        if actual != part['sha256']: raise ValueError('Local archive checksum mismatch: ' + path.name)
        for attempt in range(4):
            assets = initial_assets if attempt == 0 else api.request(prefix + '/releases/%s/assets?per_page=100' % release['id'])
            existing = next((x for x in assets if x['name'] == path.name), None)
            if existing:
                if existing.get('state') == 'uploaded' and existing['size'] == part['bytes'] and existing.get('digest') == 'sha256:' + actual:
                    break
                if release['draft'] and existing.get('state') == 'starter':
                    api.request(prefix + '/releases/assets/%s' % existing['id'], 'DELETE')
                else:
                    raise ValueError('Existing remote asset differs or is incomplete: ' + path.name)
            if not release['draft']: raise ValueError('Refusing to modify a published source release')
            try:
                result = api.upload(release['upload_url'], path)
                if result.get('digest') != 'sha256:' + actual: raise ValueError('Remote archive checksum mismatch')
                break
            except (OSError, http.client.HTTPException) as error:
                if attempt == 3: raise
                print('RETRY', path.name, 'attempt', attempt + 2, type(error).__name__, flush=True)
                time.sleep(3)
        print('UPLOADED', path.name, flush=True)
    with ThreadPoolExecutor(max_workers=a.workers) as pool:
        list(pool.map(upload_part, parts))
    # Publish only after every future completed and GitHub confirms all hashes.
    assets = {x['name']: x for x in api.request(prefix + '/releases/%s/assets?per_page=100' % release['id'])}
    for part in parts:
        remote = assets.get(part['name'], {})
        if remote.get('state') != 'uploaded' or remote.get('size') != part['bytes'] or remote.get('digest') != 'sha256:' + part['sha256']:
            raise ValueError('Release inventory is incomplete')
    if release['draft']:
        api.request(prefix + '/releases/%s' % release['id'], 'PATCH', dict(draft=False))
    print('PUBLISHED', release['html_url'], flush=True)


if __name__ == '__main__': main()
