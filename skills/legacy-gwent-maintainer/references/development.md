# Local development

Last verified: 2026-08-06

## Toolchain

- Setup: `scripts/setup-dev.ps1` installs the .NET 10 SDK, MongoDB 4.4.29,
  and Unity 2019.4.1f1 under `%LOCALAPPDATA%\LegacyGwentDev`.
- Stable profile: `scripts/start-dev.ps1` uses server 5005, MongoDB 28020, and
  connection-URI suffix `gwent-diy`.
- Isolated profile: `scripts/start-ai-dev.ps1` uses server 5010, MongoDB 28021,
  URI suffix `gwent-diy-ai`, and a separate data directory.
- The server ignores those URI suffixes when selecting repositories: game data
  is in logical database `gwentdiy`, while DIY-page data is in `Web`.
- Stop with the matching `stop-dev.ps1` or `stop-ai-dev.ps1`.
- `-FeatureManifest` accepts a path relative to the caller's working directory;
  `start-dev.ps1` resolves and validates it before launching the server from the
  project directory. Keep that resolution step: passing the caller-relative
  value directly makes `GameFeatureService` combine it with the server content
  root, silently exercising the Unity fallback instead of the intended manifest.

## Unity

- `scripts/open-unity.ps1` builds/synchronizes the Common DLL and sets
  `GWENT_SERVER_URL` only for the launched Unity process.
- For the deployed DIY-AI track, use the tracked 5010 endpoint (currently the
  direct public IP). Use the hostname only where DNS and proxy routing are known
  to preserve the real public address.
- Packaged `diy-ai` clients resolve the endpoint in this order:
  `GWENT_SERVER_URL`, `Assets/Resources/ServerEndpoint.txt`, then the compiled
  5010 fallback. The tracked resource currently uses the direct public IP to
  avoid fake-IP proxies intercepting the nonstandard port.
- Android uses package ID `cynthia.diy.ai.card`. Its Gradle postprocessor adds
  `INTERNET` and permits cleartext because 5010 does not yet provide TLS.
- `ProjectSettings.bundleVersion` and `AndroidBundleVersionCode` are coupled by
  `major * 1,000,000 + minor * 1,000 + patch`; version 2.1.9 therefore uses
  Android code 2001009. Mobile and release CI recompute and assert the tracked
  value, pass both values explicitly to GameCI, and inspect the generated APK.
- Mobile CI requires a full `expected_sha` dispatch input and rejects the build
  after checkout when `HEAD` differs. Dispatch only a pushed trusted repository
  branch, then confirm the recorded Actions `headSha` before accepting artifacts.
- Mobile CI currently produces an Android Debug-signed APK. `apksigner` reports
  the certificate for each artifact but does not treat that ephemeral identity
  as stable. Configure a dedicated keystore through protected CI secrets before
  trusted distribution; never add the key or password to the repository.
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
- Unity workflows build `Cynthia.Card.Common` directly. Keep Common and AI on
  `netstandard2.0`; do not make client builds depend on the `net10.0` server.
- The .NET 10 server accepts a SignalR 5.0.8 client handshake. This was verified
  against `/hub/gwent` after the framework migration, but a full Unity gameplay
  smoke is still required before production rollout.
- Confirm the actual TCP peer after switching endpoints; a working UI does not
  prove the client is local.

## Checks

- Build the server project to an independent output directory when a running
  service locks `bin/Debug/net10.0/Cynthia.Card.Server.dll`.
- Run PowerShell parser checks for changed `.ps1` files and `bash -n` for changed
  deployment scripts.
- `LOCAL_DEVELOPMENT.md` is the user-facing command reference.

## Website preview data

- The redesign preview runs on `http://127.0.0.1:5020` and uses the isolated
  Mongo process on 28021. Do not point it at stable 28020.
- For data-rich visual QA, use a separate disposable Mongo instance (currently
  28022). Seed only sanitized match/rank/workshop fields; never copy the remote
  `user.PassWord` field or other credentials into the local preview database.
- Before judging a UI fix, resolve the exact listening process for the viewed
  port and inspect its executable/output path. A stale 5020 `netcoreapp3.0`
  process can coexist with a newer publish on 5021 and make fixed CSS look
  broken; navigate the review tab to the verified current preview explicitly.
- The legacy `netcoreapp3.0` target cannot hot-apply managed edits or refresh an
  open browser. When watch reports `Restart is needed`/`ENC0097`, restart only
  the 5020 preview process; after Razor/CSS changes recover health, manually
  reload the tab because static assets are never injected into the open page.
- A background preview stores the `dotnet watch` root PID. Stop it with
  `scripts/stop-dev.ps1 -KeepMongoDB`, which validates and terminates the whole
  descendant watch tree; killing only the current 5020 listener leaves stale
  watcher processes behind.
- A remote seed can be restored without exposing MongoDB by opening a temporary
  authenticated SSH reverse tunnel and running `mongorestore` remotely into the
  local endpoint. Close the tunnel afterward and compare per-collection counts
  for both `gwentdiy` and `Web`.
- Temporary QA accounts/cards must use distinctive identifiers, be deleted after
  the browser flow, and be verified absent in MongoDB.
