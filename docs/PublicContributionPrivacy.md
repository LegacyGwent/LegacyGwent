# Public contribution hygiene

Use a repository-relative path or `<repo>` in published documentation. Do not
publish local user directories, workstation names, private network addresses,
personal contact details, raw diagnostic dumps, account data or credentials.
Review image/audio metadata and archive member names before releasing assets.
Keep original third-party copyright and attribution notices intact.

Use the contributor's GitHub-provided noreply address for new commits when a
private contact email should not be published. Check both author and committer
fields. A noreply address protects the contact mailbox; it still identifies a
GitHub account. Repository ownership, releases, pull requests and public activity
also remain visible. Public hosting does not provide anonymity.

Run `python scripts/check-public-content.py --base <base-commit>` before pushing.
For staged/working changes, add `--working-tree` after staging newly added files.
The CI guard checks newly added text for common personal-profile paths, private
key headers and GitHub token patterns. It withholds matched values from output.
This narrow guard does not prove the absence of all personal information or
secrets, and does not inspect binary metadata, old branches or Git identity.

Keep signing keys and license material outside Git and use encrypted repository
Secrets for CI. Do not copy secrets into issues, reports or chat transcripts.

Deleting a line in a new commit does not remove old commits, tags, other branches
or copies already downloaded by others. Historical cleanup requires reviewing
every affected reference and coordinating any history rewrite with collaborators.
Never force-push a shared branch or replace release tags without reviewing the
specific impact. See [GitHub's sensitive-data removal guidance](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/removing-sensitive-data-from-a-repository).

Asset ownership and distribution are covered separately by [ASSET_NOTICE.md](../ASSET_NOTICE.md).
Privacy measures do not establish permission to use third-party assets.
