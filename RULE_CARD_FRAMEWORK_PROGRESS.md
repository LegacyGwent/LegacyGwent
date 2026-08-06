# DIY-AI rule framework progress

Last updated: 2026-08-07

This file is the restart handoff for the local-only rule-card/client-flexibility
goal. The framework must not be deployed until the remaining visual acceptance
pass is complete. Production card batches continue to ship independently from
the pre-framework `diy-ai` line.

## Production baseline at handoff

- The independent card batch `dc373d3a81a13baf390c59087a4a43cbfb68bb95`
  is deployed on 5010. `/healthz` returns HTTP 200/`Healthy`, the service journal
  has no warning-or-higher entries since deployment, and 5005 remains active.
- Its database migration completed with a recoverable backup under
  `/var/backups/legacy-gwent/diy-ai-card-reset/20260806T172504Z.ZnkYHW`.
  Exact post-checks report zero retired card `70193` copies in decks and
  blacklists and zero newly invalid decks.
- CardMap is `1.0.0.175`. The local rule-framework commits below have not been
  included in that production release.

## Current local state

- Worktree: `card-pool-reset-worktree`
- Branch: `reset-diy-ai-card-pool`
- Local server: `http://127.0.0.1:5010`, isolated MongoDB on `28021`
- Local manifest: `game-features.rule-ui.local.json`; player rule-card entry is
  intentionally enabled for local review and AI0-AI5 are exposed.
- Latest Windows review build (includes the final ordinary-card incremental row
  refresh pass):
  `Builds/AITest-Windows-rule-local-final/DiyGwent-AITest.exe`
- The full Addressables/player build passed under Unity 2019.4.1f1. Use the
  verified short project junction `C:\gwent-ai-unity` for subsequent full builds
  because the normal worktree path exceeds legacy SBP cache path limits.

## Implemented in the current pass

- Server projections now return a reusable resolved-rule snapshot. Ordinary
  card clicks are evaluated locally; only leader/rule changes contact the server.
- Grey cards ignore clicks. Ordinary card add/remove no longer rebuilds the card
  grid. Rule changes retain the explicit cleanup preview.
- Empty, incomplete, retired-card, and otherwise invalid ordinary decks can be
  saved and exited as drafts; strict validation still blocks matchmaking.
- Deck filters persist instead of recreating all three buttons on every switch.
- Editor and match lists share a card-shaped, distinct rule-card visual treatment.
- Rule-deck tooltips follow the pointer and flip/clamp at screen edges.
- A localized `minimum N` badge appears only when the resolved minimum deck size
  differs from the ordinary 25-card minimum.
- Mode rows/launcher have stable non-scaling hover treatment; the local manifest
  exposes all six available AI profiles.
- Rule/resource/card-marker HUD was lowered beneath card details and menus;
  marker readability and tooltips were restyled and pointer-following.
- Card details use `关闭`/`返回` rather than the unrelated login label.
- Server message bodies can use `loc:` keys; surrender is no longer a hard-coded
  Chinese/English pair.
- Imported accounts show only the newest pending season result and acknowledge
  the entire stale season backlog after confirmation, preventing one old season
  popup on every login.
- Durable UX/architecture/build pitfalls are recorded under
  `skills/legacy-gwent-maintainer/references/`.

## Verification completed

- `Cynthia.Card.Server` Release build: passed, 0 warnings / 0 errors.
- `RuleCardArchitectureTests`: 36/36 passed.
- Unity 2019 script compilation: passed.
- Full Addressables + Windows player build: passed; reported player size
  `953,339,443` bytes.
- Final player-only rebuild after the incremental editor patch: passed; reported
  player size `953,341,491` bytes.
- The previous review player starts successfully against local 5010. The final
  build is compiled and packaged but still needs the visual acceptance pass below.

## Required next-session acceptance

1. Use the latest review build, not `AITest-Windows-rule-local`, and visually
   verify the rule-card header/right padding, shared editor/match rule styling,
   tooltip following, and non-flashing filters.
2. Rapidly add/remove ordinary and grey cards at several scroll positions. Confirm
   no RPC-driven pause, no popup, no card-grid rebuild, and no scroll reset.
3. Add/remove conflicting rules and verify the one-time cleanup preview, declining
   and accepting it, then leave with an incomplete draft.
4. Verify `minimum N` is hidden at 25 and visible for custom minima including zero.
5. Verify mode launcher hover and AI0-AI5 list, password matching, and server-hidden
   player-rule mode behavior.
6. Verify card-detail/menu layering over rule/resource/marker HUD and inspect one
   and many resources/markers plus their moving tooltips.
7. Re-login twice after acknowledging a season result; it must not reappear.
8. Recheck Dana Meadbh's rebuilt leader slot for the right-edge white strip.
9. Archive approved screenshots in a dedicated review directory and run the full
   server/gameplay test suites plus a local live AI match before considering a
   publishable client.
