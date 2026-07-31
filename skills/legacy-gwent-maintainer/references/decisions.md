# Active decisions

Last verified: 2026-07-31

## Isolate aggressive maintenance

Keep `diy-ai` separate from stable `diy` at every stateful boundary: branch,
port, service, Mongo process, database, data directory, and release directory.
Never auto-merge or auto-deploy DIY-AI into stable DIY.

## Seed isolated state through an explicit snapshot

DIY-AI may be refreshed from stable only through the guarded one-shot sync
command. It backs up and replaces the isolated Mongo process; it is not ongoing
replication. Stable remains online, so the result is accepted as a best-effort
seed and the two tracks diverge independently afterward.

## Prefer native atomic releases on the legacy host

Build in a pinned container in GitHub Actions, but deploy framework-dependent
publish artifacts to native systemd. This matches the existing host, avoids
requiring a container registry credential, and permits symlink rollback.

## Keep project knowledge versioned and progressively disclosed

The canonical skill lives in this repository. A personal Codex installation may
link to it for discovery, but updates must be committed here. The index routes
agents to small domain references; learning rewrites canonical truth instead of
growing a task diary.
