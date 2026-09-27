# Local premium packaging pitfalls

Last verified: 2026-09-28

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
  v1/v2 signatures, and the same certificate. No connected device was available
  for installation or rendered animation acceptance.
## Windows launcher waits after Unity has exited

- Symptom: Unity logs exit 0 and the player passes content verification, but the
  PowerShell build driver stays alive before its verification step.
- Cause: Start-Process -Wait waits for the process tree, including surviving
  helper processes. The build needs to wait only for its launched Unity process.
- Fix: retain the returned Process and call WaitForExit(), then check ExitCode
  and the actual artifact. Do not terminate unrelated Unity licensing services.
- Verification: the Windows artifact passed premium and version checks; the
  revised launcher completed Android build and artifact checks without hanging.
