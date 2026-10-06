# Android login and IL2CPP serialization

Last verified: 2026-10-07

## Login works but Addressables images are blank on IL2CPP

- Symptom: authentication and the main scene succeed, but avatar/reward art is
  blank. `MissingMethodException: Default constructor not found` names
  `ProviderOperation<ResourceManagerRuntimeData>` or `CompletedOperation<Sprite>`
  through `Activator.CreateInstance`, `LRUCacheAllocationStrategy.New` and
  `ResourceManager.CreateOperation`. This is constructor stripping, a different
  signature from System.Text.Json's missing AOT code below.
- Cause: pinned Addressables 1.18.19 reflectively allocates its internal provider,
  completed, chain, group and instance operations. The tracked `Assets/link.xml`
  did not preserve either Addressables runtime assembly. The generated Android
  `Library/com.unity.addressables/aa/Android/AddressablesLink/link.xml` exists,
  but the observed Unity 2019.4 linker command explicitly includes only the
  tracked descriptor plus engine/temporary descriptors; existence of generated
  content metadata alone does not establish constructor preservation.
- Fix: preserve `Unity.ResourceManager` and `Unity.Addressables` in the tracked
  descriptor for all player variants. This covers reflection-based operation
  allocation and catalog/provider initialization at their common linker boundary,
  without Sprite/account-specific code or changes to UI asset loading.
  Unity 2019.4's [link XML contract](https://docs.unity3d.com/2019.4/Documentation/Manual/ManagedCodeStripping.html#LinkXML)
  defines assembly-level `preserve="all"` as preserving the entire assembly.
- Prevention: inspect the exact exception, package allocation implementation and
  actual linker inputs. Do not edit ignored generated descriptors or treat a
  successful build as runtime acceptance. Assembly preservation can increase
  player size; it does not guarantee every future value-type generic AOT shape.
- Verification: the pre-fix `3fbbc738c` Android emulator reproduced both named
  constructor failures after accepted login. That cacheD APK also omitted its
  entire static `assets/aa` payload after an Addressables preprocessing move
  exception; the artifact contract and separate fix are in `premium-delivery.md`.
  Restoring payload and preserving constructors are independent requirements.
  The final `d5edd7df9` selective-cache APK passed in-place installation and
  login/main-scene loading on the API35 translated-ARM64 AVD. A disposable fresh
  registration/login rendered and confirmed Yen/Triss first rewards, then emptied
  the new-reward arrays. Card sprites and deck editor rendered, with no exceptions
  in the final observed session. The exact unavailable-address completion path
  and border rendering were not separately observed. Physical-phone behavior and
  future reflected generic shapes remain unverified; emulator success is bounded
  to the exact APK recorded in `premium-delivery.md`.

## SignalR login closes with missing AOT code

- Symptom: Android IL2CPP disconnects during login while Windows Mono works.
  The diagnostic signature is `ExecutionEngineException` with
  `no ahead of time (AOT) code was generated`, naming a closed generic
  System.Text.Json converter constructor. This is different from a native
  process crash or a connection `TimeoutException`.
- Cause: preserving System.Text.Json in `link.xml` keeps its metadata but does
  not generate every closed generic constructor discovered by reflection.
  Observed missing types were `Dictionary<string,int>`, `Nullable<bool>`, and
  `int[]`; login/profile and premium DTOs share these shapes.
- Fix: register explicit typed converters in the shared Unity SignalR JSON
  configuration (`Bootstrapper.cs` and `AOThelper.cs`). The premium deck
  selection dictionary has an explicit converter too. All player variants
  must use the same registrations rather than patching one account or RPC.
- Prevention: inspect the exact missing method signature before adding code.
  Editor/Mono tests and successful APK compilation cannot prove IL2CPP AOT
  coverage. Do not change signing keys or server account rules to treat this
  client serialization error.
- Verification: the `e51bda1ce` ARM64 premium APK successfully registered a
  disposable account and completed authentication, premium collection and
  daily quest refresh, then loaded the main scene in the API35 translated-ARM64
  emulator on October 6. No AOT exception occurred in this login. Main-scene
  trinket errors were a separate defect; phone-native crashes, later match
  RPCs and all device models were not proven by this smoke test.

## A fix exists in source but the installed package is ambiguous

- Symptom: players still report the old failure after a fix was pushed, and
  several diagnostic/rebuilt APKs all display the same `2.1.9` version.
- Cause: a source branch or local build is not a delivered player artifact.
  The file name, upload date, and version alone do not distinguish the earlier
  diagnostic APK from the later AOT fix. The premium AOT commits were initially
  only on `integrate/pr824-premium-sep26`, not the main DIY-AI source.
- Fix: integrate the runtime fix into the common client source, give the next
  hotfix a distinct native version/code, and deliver a verified artifact with
  source SHA and SHA-256 receipt plus a usable download location.
- Prevention: keep build, emulator/phone verification, and publication evidence
  separate. Never claim a community member has the fixed APK from a screenshot
  of a truncated file name.
- Verification: inspect the installed APK or its hash against the published
  receipt; repeat new-account login using that exact build. Local file links
  are useful on the maintainer's computer but are not public download URLs.
