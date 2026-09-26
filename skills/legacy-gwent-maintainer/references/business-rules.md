# Business rules

Last verified: 2026-09-26

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

## Premium cards and rewards

- Premium presentation preserves the original card ID and adds `IsPremium` on
  the wire. `DeckModel.PremiumCards` and `PremiumLeader` are nullable: missing
  values identify an old caller and must preserve, then reconcile, any stored
  selection rather than clearing it.
- Reconcile against per-copy ownership before creating the match snapshot,
  including old decks whose appearance fields are absent. An owned card ID is
  not permission to render all three copper copies as premium. Old-client edits
  immediately trim preserved selections to remaining copies and clear premium
  leader state when the leader changes; background persistence must agree.
- Card version ownership follows the effect source: a transformation or a card
  effect that generates a derived card inherits `IsPremium` from the card that
  caused the action, and a self-transformation uses itself as the source. Only
  system-owned creation with no source falls back to the account's premium
  ownership of that card ID.
- Keep premium deck selections out of `UserInfo.Decks`. Store them under
  `premium_collection.DeckSelections`; the server BSON map must continue to
  ignore premium `DeckModel` members so an older server can read the account
  database after rollback.
- Deck IDs are caller-owned strings, not necessarily GUIDs. Persist GUID keys
  unchanged and encode other IDs with `PremiumDeckStorageKey`; decode exactly
  once on freshly read wallet documents, never on already decoded results.
  Permanent invalid writes must leave the global appearance queue; only
  transient failures retry. RewardSystemTest exercises legacy IDs containing
  dots, dollar signs and the encoding prefix through the real hub and MongoDB.
- Old clients do not request the premium wallet. Login, registration, ordinary
  deck writes, matchmaking, and round progression must remain available when
  the wallet database or reward notification fails.
- Initial powder is server-owned and configured by `InitialPowder.json`
  (`Amount` 3000). `Enabled` pauses without consuming eligibility; the stable
  receipt `initial-meteorite-powder-v1` and `InitialPowderGranted` flag make
  both old-account backfill and new-account grants idempotent. Changing
  `Amount` is safe as long as that receipt ID and flag stay unchanged: only
  accounts that have not claimed yet receive the new value, and already-granted
  wallets are never deducted or re-granted. Do not add a client grant.
- Initial-grant scans repeat in the background, so skip already-claimed wallets
  in 512-user batches instead of issuing grant updates for every account on
  every sweep. Claimed wallets with missing/null ledgers still need repair;
  receipt-only legacy claims must promote the flag without another payment.
  The Mongo command-count regression verifies a converged scan performs zero
  writes and reads proportional to batches, while newly added users still grant.
- Premium crafting prices are server-owned by `PremiumCrafting.json`: Copper
  100, Silver 400, Gold 800, Leader 1000. The server publishes the per-card
  `Costs` map in `PremiumCollectionResult`; the Unity client holds no price
  fallback and only renders the returned map, so a client cannot supply or
  override a price. Changing a price does not invalidate stored craft receipts
  (`CraftReceipts.RequestId`), which remain the idempotency key.
- Daily rewards use the China calendar day: login grants 20 powder and crown
  thresholds 2/4/6 grant 25/35/45. Connected users cross midnight without
  relogging. Keep all processed round IDs across daily resets; delayed or
  repeated settlement must never pay the same round twice.
- Round rewards are background work. Persist a `daily_round_reward_jobs` entry
  before changing the wallet, retry incomplete jobs, and use the stable
  match/round key as the idempotency key. SignalR notification is best effort
  and must not control game progression or the reward commit.

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

- Run `scripts/card_art_inventory.ps1` from the skill directory; it compares
  full-size Addressable art, `GwentMap` dictionary keys and art IDs, explicit
  `CardEffectId` attributes, and `_slot` miniatures.
- Verified inventory: 1,343 unique full-size card-art assets, 715 `CardMap`
  entries, 698 explicit card-effect IDs, and 671 unique art IDs assigned to a
  card definition.
- Of the art assets, 659 are used by a card with an explicit effect, 12 are
  assigned only to effectless card definitions, and 672 are completely
  unassigned. Therefore 684 have no explicit effect, but only 672 are cleanly
  free for a new card without reusing an existing definition's art.
- Of the 672 unassigned art assets, 539 already have a matching `_slot`
  miniature and 133 need a miniature before the deck-list UI can use them
  cleanly.
- The `/cardart` authoring page enumerates a separate set of 1,049 small
  `wwwroot/scale` previews. It has 531 IDs not referenced by `CardMap`, but 499
  of those lack a Unity full-size image; only 32 overlap the clean full-art pool,
  and only one of those already has a registered miniature. Do not equate an
  available web preview with a client-ready card-art chain.
- Count occupation by the dictionary key, not the duplicated `GwentCard.CardId`
  property. Entry key `70108` currently has the incorrect property value
  `70106`; treating the property as identity miscounts effect/art ownership.
