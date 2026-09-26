#!/usr/bin/env python3
"""Create/reuse a local release key and configure encrypted GitHub Actions secrets.

Requires PyNaCl only for --upload. The private backup stays outside the repository.
"""
import argparse
import base64
import hashlib
import importlib.util
import json
import os
from pathlib import Path
import secrets
import subprocess


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--repository', required=True)
    p.add_argument('--keytool', type=Path, required=True)
    p.add_argument('--directory', type=Path, required=True)
    p.add_argument('--upload', action='store_true')
    a = p.parse_args(); folder = a.directory.resolve()
    root = Path(__file__).resolve().parents[1]
    if folder == root or root in folder.parents:
        raise ValueError('Signing backup must be outside the Git checkout')
    folder.mkdir(parents=True, exist_ok=True)
    if os.name == 'nt':
        sid = subprocess.check_output(['whoami', '/user', '/fo', 'csv', '/nh'], text=True).strip().split(',')[-1].strip('"')
        subprocess.run(['icacls', str(folder), '/inheritance:r', '/grant:r', '*' + sid + ':(OI)(CI)F'], check=True, capture_output=True)
    else: folder.chmod(0o700)
    settings = folder / 'signing.json'; key = folder / 'legacygwent-ai.keystore'
    if settings.exists():
        config = json.loads(settings.read_text())
        if config['repository'] != a.repository: raise ValueError('Signing backup belongs to another repository')
        if not key.exists(): raise ValueError('Existing signing configuration is missing its key; restore backup')
    else:
        if key.exists(): raise ValueError('Key exists without configuration; refusing to replace it')
        config = dict(repository=a.repository, alias='legacygwent-ai', password=secrets.token_urlsafe(40))
        settings.write_text(json.dumps(config), encoding='utf-8')
        env = dict(os.environ, LEGACY_GWENT_KEY_PASSWORD=config['password'])
        result = subprocess.run([str(a.keytool), '-genkeypair', '-keystore', str(key), '-storetype', 'JKS',
             '-storepass:env', 'LEGACY_GWENT_KEY_PASSWORD', '-keypass:env', 'LEGACY_GWENT_KEY_PASSWORD',
             '-alias', config['alias'], '-keyalg', 'RSA', '-keysize', '3072', '-validity', '10000',
             '-dname', 'CN=LegacyGwent AI, OU=' + a.repository.split('/')[0], '-noprompt'],
             env=env, capture_output=True)
        if result.returncode: raise RuntimeError('keytool key generation failed; private backup retained for recovery')
    env = dict(os.environ, LEGACY_GWENT_KEY_PASSWORD=config['password'])
    cert = subprocess.check_output([str(a.keytool), '-exportcert', '-keystore', str(key), '-alias', config['alias'],
                '-storepass:env', 'LEGACY_GWENT_KEY_PASSWORD'], env=env, stderr=subprocess.DEVNULL)
    fingerprint = hashlib.sha256(cert).hexdigest()
    (folder / 'certificate-sha256.txt').write_text(fingerprint + '\n')
    if a.upload:
        from nacl.public import PublicKey, SealedBox
        spec = importlib.util.spec_from_file_location('publisher', Path(__file__).with_name('publish-premium-content.py'))
        publisher = importlib.util.module_from_spec(spec); spec.loader.exec_module(publisher)
        api = publisher.GitHub(publisher.credential(a.repository.split('/')[0]))
        prefix = 'repos/' + a.repository + '/actions/secrets/'
        public = api.request(prefix + 'public-key')
        box = SealedBox(PublicKey(base64.b64decode(public['key'])))
        values = dict(ANDROID_KEYSTORE_BASE64=base64.b64encode(key.read_bytes()).decode(),
                      ANDROID_KEYSTORE_PASSWORD=config['password'], ANDROID_KEY_ALIAS=config['alias'],
                      ANDROID_KEY_PASSWORD=config['password'], ANDROID_CERT_SHA256=fingerprint)
        for name, value in values.items():
            encrypted = base64.b64encode(box.encrypt(value.encode())).decode()
            api.request(prefix + name, 'PUT', dict(encrypted_value=encrypted, key_id=public['key_id']))
            print('CONFIGURED', name, flush=True)
    print('SIGNING_BACKUP', folder)
    print('CERTIFICATE_SHA256', fingerprint)


if __name__ == '__main__': main()
