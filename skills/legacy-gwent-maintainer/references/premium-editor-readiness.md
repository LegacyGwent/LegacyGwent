# Premium Editor readiness

Last verified: 2026-10-02

## Cards become static again after a source edit or a temporary preview

- Symptom: Editor opens and the main UI works, but premium cards remain static.
  A previous content-authoring preview worked only while a temporary switch was on.
- Cause: Content/Shaders changes correctly invalidate the editor-ready marker.
  The old runtime required that marker for bundles and defaulted source loading
  to false, so restoring the temporary switch returned the main UI to static art.
- Fix: the interactive Editor uses `DynamicCardEditorContentPolicy` Automatic
  mode: validated bundles first, complete source content otherwise. The policy
  is distinct from user quality, account entitlement and packaged capability.
- Prevention: use `Tools > Dynamic Cards > Preview Status` and the workflow in
  `docs/PremiumEditorReadiness.md`. Never fabricate `.editor-ready`; never call a
  source-only preview a rebuilt-bundle or Player acceptance. Source loads remain
  synchronous; keep the existing visible-card queue and use BundlesOnly for
  performance measurements. Old bool false restores Automatic rather than
  disabling the default source fallback. Batch Automatic remains BundlesOnly.
- Verification: run the production policy fixtures, inspect the actual chosen
  source and sample multiple frames in the main collection UI. Repeat after a
  Play Mode restart. Confirm explicit quality Off remains Off. Separate these
  checks from a fresh packaged client and target-device acceptance.
  Unity 2019.4.41f2 verification passed 27 policy cases and main-UI three-frame
  captures of owned premium cards 14003 and 12011, before and after a Play Mode
  restart, with Automatic/source/ready-missing and Medium quality; both captures
  recorded advancing animation and zero new errors. This does not cover every
  card or prove the separate VFX branch's visual fixes exist on another branch.

## Resource state and quality are separate

- `ClientContent` grants Editor authoring capability, but the user's
  `DynamicCards.Quality` / legacy Enabled values still control playback.
- Default/fresh quality is Off under the existing product contract. Inspect the
  actual Unity preference before inferring a resource failure from a static card.
  Enable a quality tier when the user requests animated playback; do not make a
  cache recovery silently overwrite an explicit Off setting.
- A new clone needs the versioned premium source restore, not just the tracked
  catalog. The restore has clean-directory and integrity guards. Reuse them.
- A valid catalog may include source scenes with empty art-ID mappings. The
  current 677-scene catalog includes 13 such entries; do not reject the entire
  catalog for them. Require unique mapped art IDs and at least one mapping,
  and still check every source prefab and bundle-index entry.
- Local test authentication must resolve the existing data environment first;
  follow `development.md` and avoid logging or publishing fixture passwords.
- Collection previews require owned premium copies. Setting `IsPremium=true`
  alone does not bypass `PremiumCollectionClient.Show`; select owned samples
  for playback tests rather than changing account ownership.
