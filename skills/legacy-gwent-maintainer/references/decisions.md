# Active decisions

Last verified: 2026-07-31

## Isolate aggressive maintenance

Keep `diy-ai` separate from stable `diy` at every stateful boundary: branch,
port, service, Mongo process, database, data directory, and release directory.
Never auto-merge or auto-deploy DIY-AI into stable DIY.

## Prefer native atomic releases on the legacy host

Build in a pinned container in GitHub Actions, but deploy framework-dependent
publish artifacts to native systemd. This matches the existing host, avoids
requiring a container registry credential, and permits symlink rollback.

## Keep project knowledge versioned and progressively disclosed

The canonical skill lives in this repository. A personal Codex installation may
link to it for discovery, but updates must be committed here. The index routes
agents to small domain references; learning rewrites canonical truth instead of
growing a task diary.
