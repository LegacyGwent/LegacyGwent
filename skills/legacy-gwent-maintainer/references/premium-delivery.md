# Premium source delivery and package variants

Last verified: 2026-09-20

## Clean-checkout source delivery

- Symptom: premium builds work only in the asset-rich authoring checkout.
- Cause: the 677 catalog prefabs and dependencies occupy about 33.8 GB and are
  ignored by Git; tracking a catalog alone does not deliver them.
- Fix: `build-config/premium-content.json` pins 33 ZIP release parts (about
  5.4 GB), individual sizes/hashes, and catalog hash. `premium-content.py restore`
  verifies each archive, extracts to staging, then installs into a clean Content
  directory. The original metas travel with source assets. Standard CI skips
  source restoration. The default download cache is ignored by Git.
- Prevention: reject traversal, duplicate case-insensitive paths, symlinks,
  inventory drift, existing user sources, and differing tracked catalog/meta
  before moving anything. Catalog checkout line endings are pinned for its
  byte hash; meta comparison permits only newline/trailing whitespace changes.
  Never overwrite a published release. New source changes require a new tag.
- Verification: real 33-part local restore passed; all 677 catalog prefabs exist.
  `python scripts/test-client-packaging.py` covers roundtrip and rejection paths.
  Local restore is not proof of remote availability; publishing verifies the
  final remote asset inventory before making its draft visible.

## Platform and content are independent build dimensions

- Symptom: a standard package exposes crafting it cannot render, or Android
  accidentally ships Windows bundles.
- Cause: user quality preferences were mistaken for package capability, and
  source preparation was not part of the programmatic build entry.
- Fix: `LegacyClientBuild.Build` prepares target bundles, Addressables, and
  Player. The build stamps `Resources/ClientContent.json` for runtime and an
  embedded StreamingAssets marker for verification, then restores staged files.
  Standard capability hides premium controls and omits deck appearance fields
  so the server retains account selections. Client code never grants powder.
- Prevention: distinguish standard/premium in artifacts and Library cache keys.
  Android uses IL2CPP, both ARM ABIs, GLES3, ETC2 RGBA8 and max-1024 dynamic PNGs;
  unsupported postprocess shaders are skipped. Keep one authored source library.
- Verification: `verify-client-content.py` checks the embedded target/variant,
  premium partition membership and ARM64. Fixtures prove gate behavior, not
  real APK success. `RewardClientTest` covers capability/deck/preference rules.

## Signing and runner prerequisites

- Symptom: a valid APK cannot replace an installed version, or CI fails before
  producing a player despite passing source tests.
- Cause: a different signing identity, missing Unity license, missing Android
  toolchain, or insufficient disk for source + Library + bundles + Docker.
- Fix: persist one release keystore outside Git, back it up, and configure the
  five `ANDROID_*` Secrets. `configure-android-signing.py` can upload encrypted
  Secrets using PyNaCl. Configure the separate `UNITY_LICENSE` requirement.
- Prevention: never infer private-key ownership from an old APK. Both variants
  share package identity and signing; a new fork key generally requires an
  initial uninstall of an APK signed by someone else. Keep credentials private.
  Source extraction disk checks do not guarantee total Unity build peak space.
- Verification: check real APK manifest, signer digest, ABIs, install/upgrade,
  and actual old/new client gameplay. Static Roslyn compilation does not run
  IL2CPP or compile Android shaders. Phone memory/visual acceptance remains
  required even with low-quality render targets and texture overrides.
