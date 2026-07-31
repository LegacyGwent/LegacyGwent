# Local development

Last verified: 2026-07-31

## Toolchain

- Setup: `scripts/setup-dev.ps1` installs ASP.NET Core 3.1.32, MongoDB 4.4.29,
  and Unity 2019.4.1f1 under `%LOCALAPPDATA%\LegacyGwentDev`.
- Stable profile: `scripts/start-dev.ps1` uses server 5005, MongoDB 28020, and
  database `gwent-diy`.
- Isolated profile: `scripts/start-ai-dev.ps1` uses server 5010, MongoDB 28021,
  database `gwent-diy-ai`, and a separate data directory.
- Stop with the matching `stop-dev.ps1` or `stop-ai-dev.ps1`.

## Unity

- `scripts/open-unity.ps1` builds/synchronizes the Common DLL and sets
  `GWENT_SERVER_URL` only for the launched Unity process.
- Pass `-ServerUrl http://cynthia.ovyno.com:5010` for the deployed DIY-AI track.
- Confirm the actual TCP peer after switching endpoints; a working UI does not
  prove the client is local.

## Checks

- Build the server project to an independent output directory when a running
  service locks `bin/Debug/netcoreapp3.0/Cynthia.Card.Server.dll`.
- Run PowerShell parser checks for changed `.ps1` files and `bash -n` for changed
  deployment scripts.
- `LOCAL_DEVELOPMENT.md` is the user-facing command reference.
