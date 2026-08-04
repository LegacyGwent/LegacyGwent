# Unity release pitfalls

Last verified: 2026-08-04

## Website artwork exists but the Unity card is blank

- Symptom: an Art ID is visible under server `wwwroot/scale`, yet the Unity
  client shows no full card image or leader/deck miniature after CardMap sync.
- Cause: the website, CardMap, and Unity share a logical `CardArtsId`, not a
  physical asset store. Website previews are 120x173 files; Unity requires a
  full sprite, an optional `<id>_slot` miniature, matching `.meta` GUIDs, and
  entries in `Default Local Group.asset`. The 2024-11-17 upstream asset cleanup
  (`98e12818f`) removed 495 legacy full-size `d*` images while leaving many
  website previews, creating this split.
- Fix: restore the exact historical full-size blob and `.meta`, create the
  correctly cropped miniature when absent, register both GUID/address pairs,
  and rebuild the client. Do not upscale the website thumbnail as the shipped
  card art.
- Prevention: before assigning any Art ID, check server preview, Unity full
  image, miniature, both metas, and both Addressables addresses. Restore only
  artwork actually entering the active card pool; restoring all 495 historical
  images would add about 173 MiB of source PNGs and unnecessarily enlarge client
  delivery.
- Verification: visually inspect both full sprite and miniature, confirm GUID
  uniqueness and Addressables membership, then build a client and load the card
  plus its deck/leader slot. A CardMap version bump updates metadata/locales but
  cannot add art to an already installed client.

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

## One changed card art redownloads the whole remote bundle

- Symptom: after enabling a remote Addressables catalog, changing one card image
  makes a client download roughly 172-190 MiB again.
- Cause: the existing default card-art group uses local `Pack Together`; merely
  switching its load path to remote preserves one giant platform-specific bundle.
- Fix: split assets by change frequency or immutable content pack, build remote
  content separately for every target, preserve that player's
  `addressables_content_state.bin`, upload hash-named bundles first, and publish
  the catalog last.
- Prevention: keep bootstrap UI and fallbacks local; remove `WaitForCompletion`
  from remote load paths; retain previous catalogs and bundles for rollback.
- Verification: update one test card on Windows and Android, confirm only its
  bounded content group downloads, then verify progress, retry, disk-space,
  offline fallback, and rollback behavior.
