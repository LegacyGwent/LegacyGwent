# Local development

Last verified: 2026-07-31

## Toolchain

- Setup: `scripts/setup-dev.ps1` installs ASP.NET Core 3.1.32, MongoDB 4.4.29,
  and Unity 2019.4.1f1 under `%LOCALAPPDATA%\LegacyGwentDev`.
- Stable profile: `scripts/start-dev.ps1` uses server 5005, MongoDB 28020, and
  connection-URI suffix `gwent-diy`.
- Isolated profile: `scripts/start-ai-dev.ps1` uses server 5010, MongoDB 28021,
  URI suffix `gwent-diy-ai`, and a separate data directory.
- The server ignores those URI suffixes when selecting repositories: game data
  is in logical database `gwentdiy`, while DIY-page data is in `Web`.
- Stop with the matching `stop-dev.ps1` or `stop-ai-dev.ps1`.

## Unity

- `scripts/open-unity.ps1` builds/synchronizes the Common DLL and sets
  `GWENT_SERVER_URL` only for the launched Unity process.
- Pass `-ServerUrl http://cynthia.ovyno.com:5010` for the deployed DIY-AI track.
- Packaged `diy-ai` clients resolve the endpoint in this order:
  `GWENT_SERVER_URL`, `Assets/Resources/ServerEndpoint.txt`, then the compiled
  5010 fallback. The tracked resource currently uses the direct public IP to
  avoid fake-IP proxies intercepting the nonstandard port.
- Android uses package ID `cynthia.diy.ai.card`. Its Gradle postprocessor adds
  `INTERNET` and permits cleartext because 5010 does not yet provide TLS.
- DIY-AI resolves the initial text and audio languages by filename `cn`, then
  persists manual selections in `DiyAi.TextLanguage` and
  `DiyAi.AudioLanguage`. This makes Chinese the Editor/player default without
  inheriting the stable Windows client's PlayerPrefs.
- Login announcements have separate Chinese (`GetNotes`) and fallback English
  (`GetNotesEN`) endpoints. Keep the English branch mutually exclusive with
  `language == "cn"`; otherwise the later fetch overwrites the Chinese news.
- Desktop Unity CI runs on relevant pushes; dispatch the mobile workflow for an
  Android APK. Do not expect a workflow environment variable to become a
  persistent runtime variable inside a built player.
- Confirm the actual TCP peer after switching endpoints; a working UI does not
  prove the client is local.

## Checks

- Build the server project to an independent output directory when a running
  service locks `bin/Debug/netcoreapp3.0/Cynthia.Card.Server.dll`.
- Run PowerShell parser checks for changed `.ps1` files and `bash -n` for changed
  deployment scripts.
- `LOCAL_DEVELOPMENT.md` is the user-facing command reference.
