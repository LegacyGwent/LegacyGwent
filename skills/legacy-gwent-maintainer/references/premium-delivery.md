# Premium source delivery and package variants

Last verified: 2026-09-26

## Clean-checkout source delivery

- Symptom: premium builds work only in the asset-rich authoring checkout.
- Cause: the 677 catalog prefabs and dependencies occupy about 33.8 GB and are
  ignored by Git; tracking a catalog alone does not deliver them.
- Fix: `build-config/premium-content.json` pins 33 ZIP release parts (about
  5.4 GB), individual sizes/hashes, and catalog hash. Manifest schema 2 can
  split large ZIPs into transport chunks, validating both chunk and whole-ZIP
  hashes. `split-transport` needs no source recompression. `premium-content.py restore`
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
  Published `premium-source-20260920-v1` has 62 transport files whose remote
  sizes/digests match the manifest; one unauthenticated public download also
  matched its local hash. Local restore alone is not proof of remote availability;
  publishing verifies the final remote inventory before making its draft visible.

## Resuming an interrupted source publication

- Symptom: the tag endpoint returns 404 for a draft, or an interrupted asset
  advertises its full size but is not usable; direct HTTPS uploads stall while
  ordinary API requests work through the system proxy.
- Cause: draft tag lookup can differ from release listing. A GitHub asset in
  `starter` state can report the intended full size before upload completes.
  Python `http.client` does not automatically use urllib's system proxy config.
- Fix: reuse drafts from the release list; require `uploaded`, matching size,
  and matching SHA-256. Only replace matching incomplete assets in that draft.
  Explicit HTTP CONNECT proxy handling and smaller transport chunks support
  bounded retries. Transient read failures also need retries: a connection reset
  during inventory lookup otherwise aborts that worker outside its upload retry.
  The publisher prints retry type without credential contents.
  Updating a draft must include its intended tag and target commit along with
  name/body and draft/prerelease flags. In this publication, a target-only PATCH
  changed the draft tag to `untagged-*`, causing tag-based retry to create a
  duplicate. A full-identity PATCH restored the original draft without losing
  its uploaded files. Publication now validates the returned tag and draft flag.
- Prevention: never overwrite a published source release or accept size alone.
  Keep local archives until the full remote inventory passes verification.
- Verification: an interrupted full-size `starter` asset was replaced and its
  returned digest matched; real chunk reassembly reproduced a 590 MB ZIP hash.
  Regression fixtures cover transient read retries, no blind creation retries,
  and preservation of the intended tag when a resumed draft is published.

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
- CI also runs the Mongo-backed crafting, daily, GG, same-opponent and combined
  reward suites against an isolated MongoDB 4.4.29 instance. Run the combined
  suite using `dotnet RewardSystemTest.dll <repo> <results> 127.0.0.1`, not the
  generated apphost: its configuration probes use the dotnet process path to
  launch a copied assembly. Set `REWARD_TEST_MONGO_URI` and a working
  `MONGO_SHELL`; failed administrator funding can otherwise cascade into
  misleading inventory and concurrency failures. Keep its database separate
  from all player and stable-service data.

## Signing and runner prerequisites

- Symptom: a valid APK cannot replace an installed version, or CI fails before
  producing a player despite passing source tests.
- Cause: a different signing identity, missing Unity license, missing Android
  toolchain, or insufficient disk for source + Library + bundles + Docker.
- Fix: persist one release keystore outside Git, back it up, and configure the
  five `ANDROID_*` Secrets. `configure-android-signing.py` can upload encrypted
  Secrets using PyNaCl. GameCI Personal setup documents `UNITY_LICENSE` (Hub
  generated .ulf), `UNITY_EMAIL`, and `UNITY_PASSWORD`; all are wired in CI.
  Missing repository Secrets does not mean the local editor lacks a license.
  A Hub license may exist without a .ulf at its documented default location.
- Prevention: never infer private-key ownership from an old APK. Both variants
  share package identity and signing; a new fork key generally requires an
  initial uninstall of an APK signed by someone else. Keep credentials private.
  Hosted CI frees tools before restoring Library, retains Android build-tools
  and Java, and discards verified download ZIPs after extraction. Custom runners
  retain their installed tools. Extraction disk checks do not prove build peak
  space; source, Library, bundles, Docker and player staging all occupy disk.
- Verification: check real APK manifest, signer digest, ABIs, install/upgrade,
  and actual old/new client gameplay. Static Roslyn compilation does not run
  IL2CPP or compile Android shaders. Phone memory/visual acceptance remains
  required even with low-quality render targets and texture overrides.
