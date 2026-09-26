#!/usr/bin/env python3
"""Reject obvious personal paths and credential material in newly added text.

This is a narrow publication guard, not a full secret scan or anonymity guarantee.
It deliberately reports locations and categories, never the matched values.
"""
import argparse
import re
import subprocess
import sys

RULES = {
    'personal-profile-path': re.compile(r'''(?i)(?:[a-z]:[\\/]+Users[\\/]+[^\\/\s"'<>]+|/(?:Users|home)/[^/\s"'<>]+)'''),
    'private-key': re.compile(r'-----BEGIN (?:RSA |EC |OPENSSH )?PRIVATE KEY-----'),
    'github-token': re.compile(r'(?:gh[pousr]_[A-Za-z0-9]{30,}|github_pat_[A-Za-z0-9_]{30,})'),
}


def inspect_patch(patch):
    filename = '(unknown)'
    line_number = 0
    findings = []
    for line in patch.splitlines():
        if line.startswith('+++ b/'):
            filename = line[6:]
        elif line.startswith('@@ '):
            match = re.search(r'\+(\d+)', line)
            if match:
                line_number = int(match[1])
        elif line.startswith('+') and not line.startswith('+++'):
            for kind, pattern in RULES.items():
                if pattern.search(line[1:]):
                    findings.append((filename, line_number, kind))
            line_number += 1
        elif line.startswith(' '):
            line_number += 1
    return findings


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--base', required=True)
    parser.add_argument('--target', default='HEAD')
    parser.add_argument('--working-tree', action='store_true')
    args = parser.parse_args()
    command = ['git', 'diff', '--no-ext-diff', '--no-textconv', '--unified=0', args.base]
    if not args.working_tree:
        command.append(args.target)
    command.append('--')
    patch = subprocess.check_output(command).decode('utf-8', errors='replace')
    findings = inspect_patch(patch)
    for filename, line_number, kind in findings:
        print(f'{filename}:{line_number}: review required ({kind}; value withheld)')
    if findings:
        return 1
    print('Public-content text guard passed. Binary metadata and Git identity need separate review.')
    return 0


if __name__ == '__main__':
    sys.exit(main())
