# Active decisions

Last verified: 2026-08-01

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

Build and validate with .NET 10 in GitHub Actions, but deploy a self-contained
`linux-x64` publish to native systemd. This keeps atomic symlink rollback and
avoids both a container-registry credential and a machine-wide .NET 10 runtime.
It still depends on compatible native libraries, so deployment rejects glibc
older than 2.27 before switching the active release. A stable launcher prefers
the native host but can run retained framework-dependent releases during the
migration window, preserving rollback across the ExecStart transition.
The legacy host remains in invariant globalization mode rather than receiving a
risky OS-level ICU upgrade; predefined culture names are enabled so existing
NLog formatting remains compatible.
Normal deployments are called only after both server and policy CI jobs pass;
the deployment workflow's manual dispatch is reserved for explicit recovery or
operator-directed redeployment.

## Keep the Unity transport boundary frozen during server upgrades

Framework migration applies to the ASP.NET Core server and server-side test
tools only. Common and AI remain `netstandard2.0`, Unity remains 2019.4.1f1,
and the Unity SignalR 5.0.8 assemblies are checksum-protected in DIY-AI CI.
Upgrade transport libraries only as a separate compatibility project.

## Migrate vulnerable database dependencies through an explicit checkpoint

Use MongoDB.Driver 2.30 only to expose 3.x removals, then deploy 3.9 as the
security baseline. Do not pin a modern SharpCompress underneath the older
driver. Require a zero-advisory solution audit, isolated Mongo 4.4 functional
coverage including zlib, and a real Unity regression before dependency rollout.

## Keep project knowledge versioned and progressively disclosed

The canonical skill lives in this repository. A personal Codex installation may
link to it for discovery, but updates must be committed here. The index routes
agents to small domain references; learning rewrites canonical truth instead of
growing a task diary.
