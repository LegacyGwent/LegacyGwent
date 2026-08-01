# Unity client pitfalls

Last verified: 2026-08-01

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

## CI artifact names and internal client versions diverge

- Symptom: filenames and `version.txt` say `2.1.9`, but Windows, Linux, macOS,
  and Android metadata report a generated value such as `0.0.1415`.
- Cause: GameCI's default Semantic strategy overrides Unity's
  `ProjectSettings.bundleVersion`; the old postprocessor then writes a separate
  hard-coded filename version.
- Fix: read and validate `ProjectSettings.bundleVersion`, pass it to GameCI with
  `versioning: Custom`, and make both build metadata and `version.txt` use that
  value. For Android, track and explicitly pass the monotonic code computed as
  `major * 1,000,000 + minor * 1,000 + patch` (2.1.9 is 2001009). Runtime UI
  reads `Application.version`.
- Prevention: keep ProjectSettings as the client build version source and make
  packaging fail when `version.txt`, Android native metadata, or the computed
  version code differs. Do not rely implicitly on GameCI's current derivation.
- Verification: inspect all four native metadata formats, including Android
  `versionCode` and macOS `CFBundleVersion`, not only archive names.

## A release tag can disagree with the Unity client version

- Symptom: a release named `v2.2.0` contains players whose archive metadata and
  runtime UI still report `2.1.9`.
- Cause: validating `ProjectSettings.bundleVersion` against `version.txt` proves
  internal consistency, but does not prove that a pushed Git tag names the same
  version.
- Fix: validate the requested tag against `v${PlayerSettings.bundleVersion}` in
  a prerequisite job, verify the tag resolves to the workflow commit, create a
  hidden draft, and publish it only after every platform upload succeeds.
- Prevention: require manual workflow dispatch to supply an explicit tag, and
  make every automatic or manual path fail closed on a version mismatch; reruns
  may reuse only a draft and may replace assets only while it remains a draft.
- Verification: test matching/mismatching tag and commit values, then confirm a
  failed matrix leaves no public release and a full matrix publishes once.

## Android version code is valid but in-place upgrade still fails

- Symptom: a newer APK has a higher `versionCode`, but `adb install -r` rejects it.
- Cause: the mobile workflow has no fixed keystore; debug certificates may differ
  across builders, and Android requires the same signing identity for upgrades.
  A blank GameCI `androidVersionCode` also overrides a stale tracked value by
  deriving one from the semantic version, so CI and local builds can diverge.
- Fix: configure a durable protected keystore before public distribution.
- Prevention: keep `AndroidBundleVersionCode` synchronized in ProjectSettings,
  pass it explicitly to GameCI, inspect the generated manifest, treat signing
  identity separately from version correctness, and record the expected
  certificate SHA-256 outside the repository.
- Verification: `aapt` reports the expected package/version name/version code,
  `apksigner` verifies and reports the certificate, then old/new certificate
  digests match and an actual `adb install -r` upgrade succeeds.

## Android succeeds but the verification tail fails

- Symptom: Unity and APK preparation succeed, yet the job turns red during
  metadata/signature checks; the artifact or newly built cache may be lost.
- Cause: split steps do not inherit one another's `env`; certificate labels vary
  by build-tools; `actions/cache` saves in a success-only post step by default.
- Fix: map every expected value into the verifier; gate signatures on command
  success, `Verifies`, signer count, semantic labels, and a normalized digest.
  Always upload a prepared APK. Use separate cache restore/save actions and save
  on `always()`, successful Unity build, and cache miss.
- Prevention: policy-check wiring; run `scripts/verify_android_artifact.ps1`
  and the exact CI verifier against a real downloaded APK before committing.
- Verification: malformed digest fixtures fail, the real APK passes, a failed
  verifier retains the APK, and the next failure drill must confirm cache save.

## Unity cache restores the wrong platform or is unavailable on a review branch

- Symptom: a Linux job restores a macOS `Library`, or an exact-source Android
  review build spends roughly a cold build cycle importing assets despite a
  matching-looking cache elsewhere.
- Cause: broad restore prefixes cross target platforms, while Actions caches are
  branch-scoped and a sibling branch's entry may not be visible to the review
  branch.
- Fix: include `${{ matrix.targetPlatform }}` in every cache key and restore
  prefix; let a correctly running cold build finish instead of retriggering it.
- Prevention: distinguish cache visibility from build health and retain the
  exact `expected_sha` gate for mobile dispatches.
- Verification: a cache miss never downloads another platform's archive, the
  job still reports the expected HEAD, and the produced artifact passes native
  metadata inspection.

## Native metadata or a rebuilt artifact hash appears to drift

- Symptom: Explorer reports `2019.4.1f1` instead of app version `2.1.9`, or an
  exact-source rebuild changes all archive hashes despite no client-source edit.
- Cause: Unity stamps its engine version into the Windows bootstrap; rebuilds
  also refresh PE timestamps, assembly MVID/PDB GUIDs, and Unity's build ID.
- Fix: verify `version.txt`, serialized configuration, runtime UI, and native
  metadata; compare paths, modes, unpacked sizes, and semantic IL when needed.
- Prevention: treat engine metadata and reproducibility noise separately from
  app metadata and logic; do not accept or reject a build on ZIP SHA alone.
- Verification: app sources agree; unexplained binary differences are limited
  to expected identifiers and decompiled managed code remains equivalent.

## macOS keeps a legacy placeholder bundle identifier

- Symptom: `Info.plist` contains `com.Company.ProductName` although the app name,
  endpoint, language, and version are correct.
- Cause: the legacy Unity project never assigned a production macOS bundle ID;
  changing the AITest name or version does not rewrite that independent field.
- Fix: treat the current value as a known packaging limitation; choose and test
  a durable reverse-DNS identifier before signing/notarizing public macOS builds.
- Prevention: audit bundle identity separately from `CFBundleVersion` and
  `CFBundleShortVersionString`.
- Verification: version and endpoint checks pass now; a future signed build must
  report the approved non-placeholder ID and preserve upgrade identity.

## The available Android emulator cannot run the current APK ABI

- Symptom: the local API 35 x86_64 AVD cannot install or launch an APK that
  otherwise passes manifest and signature checks.
- Cause: this AVD advertises translated arm64 support but no 32-bit ABI, while
  the current AITest APK contains only `armeabi-v7a` libraries.
- Fix: use ARMv7-capable physical hardware for this artifact, or add and verify
  `arm64-v8a` in a later client change.
- Prevention: inspect APK native libraries and `ro.product.cpu.abilist*` before
  spending time booting an emulator.
- Verification: at least one APK ABI intersects the target device ABI list, then
  installation, startup, and the actual 5010 TCP connection succeed.
