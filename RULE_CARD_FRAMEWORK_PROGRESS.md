# DIY-AI rule framework progress

Last updated: 2026-08-08

This file is the restart handoff for the local-only rule-card/client-flexibility
goal. The framework must not be deployed until the remaining visual acceptance
pass is complete. Production card batches continue to ship independently from
the pre-framework `diy-ai` line.

## Production baseline at handoff

- The latest independently verified production card batch is
  `580f469c3bdb8451d933d9608e60bbc34e2b3b90`; its DIY-AI workflow and real
  5010 deployment completed successfully. The local rule-framework commits
  below have not been included in that production release.
- Production card batches and the local rule framework remain separated. The
  remote `origin/diy-ai` currently points at `580f469c3`; no rule-framework
  commit has been pushed or deployed, and 5005 has not been touched.

## Current local state

- Worktree: `card-pool-reset-worktree`
- Branch: `reset-diy-ai-card-pool`
- Local review server: `http://127.0.0.1:5010`, isolated MongoDB on `28021`;
  both are stopped after the final review.
- Local manifest: `game-features.rule-ui.local.json`; player rule-card entry is
  intentionally enabled for local review. The production/default
  `game-features.json` remains disabled, proving the server-side kill switch
  without changing the client. AI0-AI5 and a server-authored special challenge
  fixture are exposed locally.
- Latest verified Windows review build:
  `verification-output/review11-final/DiyGwent-AITest.exe`
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
  exposes all six available AI profiles. Category headings/type badges may be
  localized and supplied by the server for future challenge/test categories.
- Combined rule fingerprints include package versions. Match results retain
  backward-compatible rule ID lists plus per-side versioned package identities.
- `OnDeckBuildingAdjust` proposals now drive projection, draft validation,
  strict mode validation, PVP keys, and runtime fingerprints through one
  authority path. A throwing experimental effect is contained and fails only
  that projection/save/match closed.
- Ordinary card queries exclude the rule zone while explicit event dispatch
  still reaches rules, preventing normal effects from selecting rule cards.
- The generated mode catalogue refreshes from the live server Manifest on every
  entry, preserves valid selection, falls back to casual when a selected mode
  disappears, and can be changed/restored without a service or client restart.
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
- `RuleCardArchitectureTests`: 44/44 passed (83 server tests total).
- Unity 2019 script compilation: passed.
- Full Addressables inventory/build proof and the final Windows Player build
  passed; the review-11 player size is `953,353,515` bytes.
- Full-art inventory now proves 1,838/1,838 files are Addressable, with zero
  mapped card art missing full-size assets. Four unassigned website previews
  remain web-only because no original full-size source exists.
- Review screenshots are archived under
  `verification-output/rule-framework-review-20260807` and the final review-7
  evidence is under `verification-output/rule-framework-review-20260807-final`.
- The review-7 player logged into the isolated 5010 service, rendered the
  server-authored `特殊挑战` category and AI0-AI5 list, and completed an AI1
  match naturally. The persisted result at `2026-08-07T11:53:26.249Z` has
  `isSurrender=false`, `ModeId=ai.1`, ruleset `local-rule-ui-1`, and the AI-side
  versioned rule package `99004@1`.
- Gameplay tests: 75/75 passed. Together with 83 server tests, the current
  automated total is 158/158. The server Release build has 0 warnings and
  0 errors.
- A real SignalR probe against the isolated 5010 service proved mode hot reload:
  eight modes initially, seven immediately after disabling `challenge.feast`,
  and eight after restoring the file, without restarting the service. The
  restored Manifest SHA-256 is
  `4123568A9CBF4332DEE7CD893A79F89E08E834EC78834029BE5657DE422A7A65`.

## Remaining visual acceptance before publication

1. Owner reviews the archived screenshots for final visual approval. The mode
   menu, special challenge, full AI list, hover state, and completed AI1 match
   have packaged-client captures; the review services and clients are stopped.
2. If requested, repeat rapid ordinary/grey-card edits and conflicting-rule
   transitions interactively with the owner; their implementation and earlier
   screenshots are already present in the main review archive.
3. Keep the feature local and unpushed until explicit publication approval.
