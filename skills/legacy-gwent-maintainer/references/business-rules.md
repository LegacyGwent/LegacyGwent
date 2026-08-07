# Business rules

Last verified: 2026-08-03

## DIY-AI release identity

- The player-facing product name is `DiyGwent AITest`. `AITest` intentionally
  lowers expectations: this is an experimental, aggressively changing track,
  not a promise that it is stronger or more stable than DIY.
- Use `DiyGwent-AITest-<platform>-<version>` for distributed client filenames.
  Keep internal branch, service, database, and deployment names as `diy-ai`.
- Preserve the distinct Unity product name as well as the Android package ID;
  the former isolates Windows PlayerPrefs/persistent data and the latter lets
  both Android clients coexist.

## Account identity

- Registration form naming is counterintuitive: stored `UserName` is the login;
  stored `PlayerName` is the visible in-game name.
- Verify account creation in MongoDB rather than trusting only the client screen.

## Deck validity

- Use `Cynthia.Card.GwentDeck.IsBasicDeck(...)` as the source of truth.
- A basic deck contains 25 through 40 cards, inclusive. The leader is stored
  separately and must not appear in that card list; the same method also limits
  golds to 4, silvers to 6, gold/silver copies to 1, and copper copies to 3.
- Saving and matchmaking have different validity boundaries. The deck editor
  may save an unfinished draft, including a leader-only deck, when it satisfies
  `IsHalfBasicDeck` or `IsHalfSpecialDeck`. Starting any match must still require
  the complete `IsBasicDeck` or `IsSpecialDeck` rule, including the 25-card
  minimum. Do not reject drafts in `AddDeck`, and do not relax matchmaking.
- After the DIY-AI card-pool reset, use `GwentDeck.CreateBasicDeck(0)` for new
  registrations and protocol probes. It returns the valid 25-card original
  starter deck. `CreateBasicDeck(1)` still contains retired card `70157` and is
  correctly rejected by the server; do not reuse the numbered legacy presets
  without auditing them against `DiyAiCardPool`.

## AI matchmaking

- Match passwords map `ai` to Geralt/Ciri (`GeraltNovaAI`), `ai1` to Recruit
  Training (`SoldierTrainAI`), `ai2` to Avallac'h mill (`MillAI`), `ai3` to
  King Auberon, `ai4` to Iron Falcon, and `ai5` to Dragon Hunter. Keep the
  Chinese and English login announcements aligned with this parser.
- `ai1` selects `SoldierTrainAI`, whose deck is based on repeated recruit card
  `89008` and whose bidding behavior is predictable.
- Forced-AI suffixes `#` and legacy `#f` are normalized by
  `GwentServerModels/GwentMatchs.cs` on `diy-ai`.
- UI instructions and server password parsing have diverged before; verify a
  match actually starts and inspect the server parser when they disagree.
- A headless SignalR 5.0.8 protocol client can reuse `SoldierTrainAI` as its
  decision engine: convert incoming `IList<Operation<int>>` values to enum-based
  operations, feed them through the AI server-operation handler, and return each
  generated user operation to the hub. Require one response per decision and a
  natural `GameEnd`; surrender is not a successful compatibility proof.

## Match verification

- A real AI result is persisted in MongoDB collection `aigameresults` with
  player names, deck names, round scores, win counts, surrender state, and
  balance point.
- In a persisted `GameResult`, the `Red*` fields belong to `RedCoin[0]`, which
  is also `GameRound` for the first turn of round one; display that side as the
  initial first mover and the `Blue*` side as the initial second mover. Do not
  infer this order from the red/blue English coin labels in legacy UI text.
- Match documents do not persist a separate faction field. Resolve each side's
  faction from its leader ID through `GwentMap.CardMap`; keep an unknown fallback
  for missing or retired leader IDs instead of indexing the map directly.
- Prefer the persisted result plus the Unity victory screen for end-to-end proof.
- Public season statistics count only ranked results for which
  `GameResult.IsEffective()` is true. Use `RedPlayerStatus()` and
  `BluePlayerStatus()` rather than inferring the opponent result from a raw
  `RedPlayerGameResultStatus`; surrender, draw, and incomplete records otherwise
  produce incorrect faction totals.

## Match process retention

- MongoDB collections `gameresults` and `aigameresults` persist only the match
  summary: identities, leaders, deck names/codes, round scores, win counts,
  result/ranked/surrender flags, MMR, blacklist codes, balance point, and time.
  They do not contain turns, chosen targets, random outcomes, or board snapshots.
- `GwentServerGame.HistoryList` is an in-memory played-card list used by card
  effects. It is neither a complete action sequence nor copied into `GameResult`.
- `ClientPlayer.OperactionList` and `Viewer.OperationList` are live SignalR
  transport buffers. Sending copies their contents to the client and clears the
  list; neither buffer is written to MongoDB.
- The 2026-08-03 DIY-AI database had no collection named for replay, operation,
  history, record, or process. Existing results cannot reconstruct a deterministic
  replay; adding one requires a new versioned event log or periodic snapshots.

## Spectator mode

- Spectator support belongs to the DIY lineage, not specifically to DIY-AI. It
  was introduced by `1f94022e3` in 2023, hidden by `c644e1e61` in 2024, and
  re-enabled and expanded by `d24f8c8f` on 2026-01-31. Consequently the
  2026-01-23 DIY 2.1.8 binary has no visible entry while later DIY and DIY-AI
  builds do; current `diy` and `diy-ai` spectator code is equivalent.
- The normal room list exposes every ready AI match, but exposes a ready
  human-versus-human match only when its password contains `#w`. This is only a
  discovery filter: `JoinViewList` rechecks login state, `Standby`, room ID and
  readiness, but does not enforce `#w` as authorization.
- A viewer is read-only in the intended client flow and receives the current
  board plus live operations from Player 1's perspective, including both
  players' complete hands. Treat it as experimental/debugging functionality,
  not a privacy boundary or a tournament-safe public spectator system.

## DIY workshop review

- Sending a design to Card Review preserves its existing votes and comments.
- Removing a proposal from Card Review resets the source `DiyCards.IsInDiscuss`
  flag so it can be revised and submitted again.
- Votes are mutually exclusive per user. Comments and votes must be written with
  Mongo atomic operators and the page must adopt the returned server document;
  never replace a whole array from a Blazor circuit snapshot.

## Card art inventory

- From the repository root, run
  `skills/legacy-gwent-maintainer/scripts/card_art_inventory.ps1`; it compares
  full-size art files, full-art Addressables, `GwentMap` dictionary keys/art IDs,
  explicit `CardEffectId` attributes, `_slot` miniatures, and web previews.
- Verified inventory (2026-08-07): 1,838 unique full-size card-art files and
  exactly 1,838 full-art Addressable entries, with zero missing or dangling
  full-art addresses. All 674 unique art IDs assigned by the 718 `CardMap`
  entries have both a full-size file and an Addressable entry.
- Of the full-size assets, 662 are used by a card with an explicit effect, 12
  are assigned only to effectless card definitions, and 1,164 are completely
  unassigned. Of those unassigned assets, 537 already have a matching `_slot`
  miniature and 627 do not.
- Seventeen mapped art IDs lack a `_slot`; most are derived/token/internal cards
  and the legacy list UI has a generic fallback. Audit the list before making
  one of them a leader or otherwise relying on its deck-list banner.
- The `/cardart` authoring page enumerates a separate set of 1,049 small
  `wwwroot/scale` previews. It has 530 IDs not referenced by `CardMap`; 526 have
  a restored Unity full-size image and four (`c10000300`, `c10001600`,
  `c10002300`, `c10003200`) are web-only previews with no original full-size
  source. Do not upscale those previews silently or call them client-ready.
- Count occupation by the dictionary key, not the duplicated
  `GwentCard.CardId` property. The inventory currently reports zero key/property
  mismatches, but retaining this check prevents historical identity drift.
