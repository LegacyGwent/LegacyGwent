# Unity client pitfalls

Last verified: 2026-08-10

## Automated mouse input misses the captured Unity control

- Symptom: a `PrintWindow` screenshot shows a control at one coordinate, but
  `SetCursorPos` clicks a different row or button even though the player is
  fullscreen and positioned at the origin.
- Cause: the captured client image is in logical pixels while Windows cursor
  APIs use physical desktop pixels under display scaling. At 125% scaling, a
  screenshot point `(x, y)` must be clicked near `(1.25x, 1.25y)`.
- Fix: obtain the real window rectangle and DPI scale, then transform screenshot
  coordinates before input. Keep a short mouse-down interval so Unity receives
  a complete click.
- Verification: click a distinctive deck row, confirm its inline buttons appear,
  and archive a screenshot before using the coordinate mapping for destructive
  or state-changing UI tests.

## A standalone player shows white card placeholders

- Symptom: the Unity player compiles and connects, but card art is white and
  `StreamingAssets/aa/settings.json` is absent or Addressables runtime data is
  null.
- Cause: `BuildPipeline.BuildPlayer` does not build Addressables content. The
  Editor can still resolve imported assets, so Editor-only testing hides this
  packaging failure.
- Fix: call `AddressableAssetSettings.BuildPlayerContent()` and fail on its
  reported error before `BuildPipeline.BuildPlayer`; then verify the packaged
  `StreamingAssets/aa` catalog and bundles exist.
- Unity 2019/SBP caveat: a deep checkout path can make BuildCache paths exceed
  legacy Windows `MAX_PATH`, sometimes leaving a corrupt `Library/BuildCache`.
  Build through a short directory junction such as `C:\\Users\\<user>\\gwai`,
  and move a proven-corrupt cache aside before retrying.
- Verification: launch the packaged player, log in, open the full-art card pool,
  and compare its Common DLL hash with the server/source build used for the test.

## A leader miniature leaves bands or leaks past the banner frame

- Symptom: a deck-list leader portrait appears as a thin horizontal strip with
  the faction backing visible above and below; after disabling aspect
  preservation, the portrait may instead extend past the right metal border.
- Cause: `LeaderMiniature` can preserve the sprite aspect ratio, so a wide slot
  sprite does not fill the roughly 4.3:1 banner. Some faction prefabs also give
  the miniature a rectangle slightly wider than the visible frame; filling that
  rectangle exposes the overflow because the leader banner has no mask.
- Fix: keep a face-focused 512x128 (4:1) `<CardArtsId>_slot` sprite, let the art
  fill its assigned rectangle, and wrap only the art image in a `RectMask2D`
  clip window derived from the immediate banner parent's visible rectangle.
  Inset the clip a few pixels inside the metal frame. Keep the frame, leader
  name, faction decoration, strength diamond, and interaction objects outside
  the mask so they remain sharp and unobstructed.
- Prevention: do not hand-tune one portrait's horizontal offset to hide an edge.
  Apply the same clipping helper in both the match deck list and deck-editor
  leader header, and preview the exact slot sprite in every faction prefab.
  Treat the full card image and `_slot` miniature as separate compositions of
  the same source artwork.
- Verification: the slot remains addressable under `<CardArtsId>_slot`, fills
  the banner vertically, retains a recognizable face-focused crop, has neither
  a pale strip nor visible overflow at either edge, and its border/decorations
  are not clipped. Verify both collapsed and expanded deck rows.

## New card art loads but its deck-list miniature is missing

- Symptom: the full card opens correctly, but deck lists or compact card rows
  cannot load `<CardArtsId>_slot` even though the PNG and `.meta` exist.
- Cause: full art and compact art belong to separate Addressables groups. The
  full image is registered in `Default Local Group.asset`, while the slot image
  must be registered independently in `Miniatures.asset` with its own GUID and
  `<CardArtsId>_slot` address.
- Fix: add both assets with their exact `.meta` GUIDs to their corresponding
  groups; do not assume placing a file under `Assets/Addressables/Miniatures`
  registers it.
- Prevention: static card-batch tests must assert both files and both group
  addresses for every new art ID.
- Verification: the group entries resolve to the two `.meta` GUIDs and the
  client can load both `<CardArtsId>` and `<CardArtsId>_slot`.

## Downloaded macOS or Linux client is not executable

- Symptom: CI is green, but the downloaded macOS app or Linux binary will not
  launch, or an app-bundle symbolic link has become an ordinary file.
- Cause: `actions/upload-artifact` normalizes permissions and does not preserve
  symlinks when it uploads a raw build directory.
- Fix: create a nested ZIP on the Ubuntu build runner with Info-ZIP `zip -y`
  before artifact upload; ZIP records the original Unix modes and link entries.
- Prevention: upload the prebuilt `DiyGwent-AITest-<platform>-<version>.zip`,
  never the raw macOS/Linux Unity output directory.
- Verification: inspect ZIP external attributes/link entries and test extraction
  plus launch on the target OS.

## Fresh or non-Windows clients keep stale card descriptions

- Symptom: gameplay uses the current server rules, but a fresh client or a
  macOS/Linux build still displays card text bundled with an older client.
- Cause: locale download was incorrectly gated on both missing files and a
  client/server CardMap mismatch; macOS/Linux also left the locale save path
  uninitialized.
- Fix: decide locale refresh from missing files or cached-locale version alone,
  default a missing version marker to `0.0.0.0`, and use
  `Application.persistentDataPath` on every non-Windows player platform.
- Prevention: keep the refresh policy in testable shared code, cover its truth
  table, assert the Unity integration and portable path branch, and increment
  `CardMapVersion` whenever delivered card names or descriptions change.
- Verification: tests cover fresh/current, fresh/stale, cached/stale,
  cached/current, and missing-marker cases; all four client builds must compile.

## Unity license material is committed in a workflow

- Symptom: a public workflow contains the complete Unity license document or
  machine-bound activation data.
- Cause: the license was stored as a repository `env` literal instead of an
  Actions secret.
- Fix: migrate the value to the repository `UNITY_LICENSE` Actions secret and
  reference `${{ secrets.UNITY_LICENSE }}` from every Unity workflow.
- Prevention: make policy CI reject inline XML license content, and never print
  or decode the secret in logs.
- Verification: `gh secret list` shows the secret name, policy finds no inline
  XML in workflows, and a review-branch Unity build can activate successfully.
- Security boundary: removing the current file does not erase Git history;
  revoke or reissue previously exposed activation material in Unity separately.

## Android client cannot reach plain HTTP 5010

- Symptom: a Windows build connects, but an Android 9+ build fails before login.
- Cause: modern target SDKs block cleartext HTTP unless the manifest opts in;
  Android also cannot use a developer machine's process environment override.
- Fix: keep the DIY-AI Android Gradle manifest postprocessor enabled until 5010
  is behind TLS, and bake the endpoint through `ServerEndpoint.txt`.
- Prevention: use distinct package ID `cynthia.diy.ai.card`, verify the generated
  manifest, and migrate the service to HTTPS before removing the opt-in.
- Verification: APK manifest contains `INTERNET` and `usesCleartextTraffic`, and
  server logs show the device connecting to 5010.

## Build-time environment does not configure a packaged Unity player

- Symptom: CI sets `GWENT_SERVER_URL`, but the downloaded client still connects
  to the branch fallback.
- Cause: the environment variable is read when the player runs, not serialized
  into the build by GitHub Actions.
- Fix: update `Assets/Resources/ServerEndpoint.txt` for packaged defaults; retain
  the environment variable for runtime desktop overrides.
- Prevention: treat endpoint assets and runtime environment settings as separate
  configuration channels.
- Verification: run the artifact without an environment override and inspect its
  actual TCP peer.

## Unity silently connects to the public server

- Symptom: local UI works, but new accounts or results do not appear in local MongoDB.
- Cause: the client historically resolved `cynthia.ovyno.com` directly.
- Fix: configure `GWENT_SERVER_URL`; use `scripts/open-unity.ps1`.
- Prevention: keep the endpoint configurable and inspect the established TCP peer.
- Verification: Unity connects to the intended loopback or DIY-AI address.

## A discarded SignalR startup task races the first invocation

- Symptom: first launch intermittently throws `InvokeCoreAsync cannot be called
  if the connection is not active`, while server logs show short WebSocket 101
  sessions without a matching server exception.
- Cause: `GwentClientService` called `HubConnection.StartAsync()` in its
  constructor and discarded the task; initialization, login, or registration
  could invoke the hub while that connection was still starting.
- Fix: remove constructor I/O and route start, stop, initialization, login, and
  registration through one awaited `SemaphoreSlim`-guarded connection lifecycle;
  propagate scene cancellation through startup and initialization RPCs.
- Prevention: constructors may register handlers but must not fire-and-forget
  network startup; cancel scene-owned retry tasks from `OnDestroy`.
- Verification: search for raw `HubConnection.StartAsync()` calls, run a batch
  compile, then prove first-launch login and a complete match in the real player.

## A healthy packaged player has no visible window in an automation shell

- Symptom: the Unity process is responsive, initializes D3D, connects to
  SignalR, and logs no exception, but `MainWindowHandle` remains zero.
- Cause: some Codex/PTY execution shells run in a non-interactive Windows
  desktop. A Notepad process launched from the same shell also has no window,
  so this is not evidence of a Unity startup regression.
- Prevention: distinguish process health from desktop visibility before editing
  client startup code. Use the interactive computer-control channel for visual
  work; use scoped window captures only, never a full-desktop capture.
- Verification: compare with a trivial GUI process in the same shell, inspect
  `Player.log` for engine/SignalR progress, and cover server-driven behavior via
  SignalR probes plus deterministic tests when the interactive channel is absent.
