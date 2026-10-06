# Local premium packaging pitfalls

Last verified: 2026-10-07

## Unity 2019 Mono exceeds Windows path limits

- Symptom: all 677 premium cards finish bundle creation, then
  DynamicCardEditorCache.WriteManifest throws DirectoryNotFoundException in
  File.OpenRead for an animation that exists on disk.
- Cause: the observed absolute path is 262 characters; old Unity Mono cannot
  read it through the long checkout path although Unity native import succeeded.
- Fix: after Unity exits, create a short directory junction pointing to the same
  checkout and invoke the local build script through that alias. Preserve the
  Library directory; do not duplicate assets or bypass manifest verification.
- Prevention: keep the full Unity asset path below legacy Windows limits.
- Verification: confirm the file exists, verify the script's UnityProjectRoot
  retains the short alias, then require successful manifest and player build.
  Both Windows and Android short-path builds exited successfully and passed
  premium artifact validation.

## Old Android JDK rejects a newer debug keystore format

- Symptom: Unity's bundled Java 8 keytool reports Invalid keystore format, while
  current Java keytool reads the existing androiddebugkey with its known password.
- Cause: keystore format compatibility, not evidence of a wrong password or key.
- Fix: use the newer keytool to import the existing alias into a separate JKS
  file outside Git. Keep the original keystore intact. Pass the compatible copy
  using GWENT_ANDROID_DEBUG_KEYSTORE to scripts/build-premium-local.ps1.
- Prevention: verify with the actual bundled JDK before queuing Android work.
- Verification: old keytool reads the JKS and modern keytool reports identical
  SHA-256 certificate fingerprints for original and converted copies. Native APK
  signature/ABI checks passed: version 2.1.9/2001009, ARMv7 and ARM64,
  v1/v2 signatures, and the same certificate. The final `d5edd7df9` APK retains
  that signing identity (`cf51` certificate prefix), ARMv7/ARM64 and native version
  2.1.10/2001010; `adb install -r` succeeded. Rendered animation acceptance is
  bounded to the API35 translated-ARM64 AVD, as recorded in `premium-delivery.md`.

## Windows launcher waits after Unity has exited

- Symptom: Unity logs exit 0 and the player passes content verification, but the
  PowerShell build driver stays alive before its verification step.
- Cause: Start-Process -Wait waits for the process tree, including surviving
  helper processes. The build needs to wait only for its launched Unity process.
- Fix: retain the returned Process and call WaitForExit(), then check ExitCode
  and the actual artifact. Do not terminate unrelated Unity licensing services.
- Verification: the Windows artifact passed premium and version checks; the
  revised launcher completed Android build and artifact checks without hanging.

## Android export exhausts the project drive despite output on another drive

- Symptom: premium bundles finish, then BuildPlayer fails with a FileVFS write
  assertion or `IOException: Win32 IO returned 112`. The latter was observed
  while copying `Temp/StagingArea/assets/bin/Data` into
  `Temp/gradleOut/unityLibrary/src/main/assets/bin/Data`; the project drive had
  no free space. This is a failed build, not evidence of a defective APK.
- Cause: Unity's project-local scratch directories duplicate player assets
  during Gradle export. Moving only the final APK destination does not relocate
  scratch files or the existing multi-platform Library cache.
- Fix: stop only after the Unity build exits, resolve and verify workspace and
  archive paths, preserve failed scratch output and old diagnostic APKs on a
  spacious drive (verify APK hashes). Preserve the Library cache, but keep its
  move-sensitive directories on the project volume and relocate only bulky
  caches using the selective layout below. Retain assets, metas, signing identity
  and the short project alias. Never delete a running build's cache.
- Prevention: check free space on both the actual project/scratch volume and
  output/cache volume before retrying; allow for simultaneous staging and
  Gradle asset copies, not just the final APK size. A cached build can still
  exhaust 11 GiB of project-drive headroom. Do not repeat that retry unchanged.
- Verification: failed builds and their files were preserved; selective cache
  relocation restored project-drive headroom, and the final `d5edd7df9` APK
  completed with static and dynamic content. Native metadata and emulator
  acceptance passed independently; whole-Library relocation is not the fix.

## Moving the whole Library breaks Unity's same-volume moves

- Symptom: after moving Library to D, a later script refresh reports copying
  `Temp/UnityEngine.UI.dll` and `Temp/UnityEngine.TestRunner.dll` into
  `Library/ScriptAssemblies` failed, followed by the generic compiler-error
  exit. Both compiled DLLs exist; there are no C# syntax diagnostics.
- Cause: entire-Library relocation separates paths which Unity and packages
  move rather than copy. Addressables 1.18.19 additionally throws
  UnauthorizedAccessException in CopyTemporaryPlayerBuildData at its explicit
  Directory.Move from Library/com.unity.addressables to Assets/StreamingAssets.
  The source resolves to D and destination to C. A Temp junction is not a
  durable workaround: Unity removes/recreates Temp during its lifecycle.
- Fix: with Unity stopped, preserve caches and restore a real project-volume
  Library directory. Keep ScriptAssemblies, com.unity.addressables and
  PlayerDataCache on the Assets/Temp volume; relocate bulky Artifacts,
  BuildCache, DynamicCardsBundles and IL2CPP caches individually via junctions.
- Prevention: audit actual rename/move pairs before moving a whole cache tree;
  final output location does not determine all package scratch destinations.
  Do not infer source compile errors from the final generic error message.
- Verification: the final `d5edd7df9` selective-cache build completed without
  ScriptAssemblies DLL-transfer, Addressables directory-move or clang failure.
  No NDK or ABI change was needed. Its APK passed the shared static/dynamic
  content gate, native metadata/signature checks, in-place install and the
  API35 translated-ARM64 AVD acceptance recorded in `premium-delivery.md`.
  This establishes the observed cache-layout remedy, not physical-phone coverage.
