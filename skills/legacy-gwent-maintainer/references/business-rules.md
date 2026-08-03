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

## Similar Chinese card names

- `CardId.DimunPirate` (`64002`) is “迪门家族海盗”: base strength 11 and
  discards all remaining copies of itself from the deck.
- `CardId.DimunCorsair` (`64028`) is “迪门家族海贼”: base strength 1 after the
  2026-08-02 balance patch and resurrects a Bronze Machine unit.
- Do not identify these cards from the shared “迪门家族海…” prefix. Confirm the
  exact Chinese name, `CardId`, art, and effect text before applying balance data.

## Selected DIY cards in the reset pool

- `70001` is 昆恩法印 and is restored as a deckable Copper Neutral spell in
  DIY-AI `1.0.0.164`. It selects a Bronze/Silver unit in hand and gives that
  physical card plus same-ID cards currently in hand/deck a server-internal
  pending Quen marker. Each marker waits for its own first successful landing on
  its controller's side after deployment, then grants Boost 2 and Shield once.
- Pending Quen is attached to each `GameCard`, not inferred later from card ID.
  Death, return to hand, banish, or movement to the enemy side during deployment
  does not consume it. A later qualifying summon/resurrection/normal play can
  consume it; transformation removes it with the replaced `EffectSet`.
- `70041` is 鬼针草煎药 and `70042` is 合欢茎魔药. DIY-AI deliberately
  makes both cards deckable while the rest of the ordinary DIY retirement
  manifest stays hidden.
- Use exactly `Special + Alchemy`; neither card has the `Item` category. This
  lets alchemy effects such as Viper Witcher count them without making them
  eligible for item-only effects.
- Both effects apply 2 points four times, with no separate initial 3-point
  hit/boost. Each copy of the counterpart potion in the player's graveyard adds
  one more 2-point repetition.

## Temporary public balance variants

- DIY-AI exposes four independent Copper IDs for each test family. The original
  unsuffixed name is the Z/original rule; A/B/C are separate cards and each has
  its own three-copy limit. Do not add a shared family limit unless requested.
- Viper Witcher uses `34022` for Z (5 power, 1 damage per starting-deck Alchemy),
  `34034` for A (5 power, 2 damage per complete group of 3 Alchemy), `34035`
  for B (5 power, base 3 damage plus 2 per complete group of 3), and `34036`
  for C (3 power with the Z effect).
- An Craite Greatsword uses `64009` for Z (8 power, 2-turn timer, Strengthen 2),
  `64035` for A (8/3 turns/2), `64036` for B (8/3 turns/3), and `64037` for C
  (7/2 turns/2). Each timer resets to the same value after triggering.
- Variants share their family's `CardArtsId`, miniature, and voice, but gameplay,
  deck limits, and same-card identity follow `CardId`, not art or display name.

## Saesenthessis: Blaze refill

- `12006` banishes the other cards remaining in its controller's hand, then
  draws that same count one card at a time.
- When the deck is empty before a required draw, snapshot every unit currently
  in that player's cemetery and return each still-eligible unit to a random deck
  position through the normal `Resurrect(... MyDeck ...)` helper. Non-unit cards
  remain in the cemetery. Continue drawing afterward; stop cleanly if both the
  deck and eligible cemetery-unit pool are empty.
- Returning the units must send `AfterCardResurrect`. This is the established
  “counts as resurrecting” behavior used by Dimun Smuggler, so resurrection
  listeners such as Tuirseach Skirmisher still trigger even though the
  destination is the deck rather than the battlefield.

## Generate-effect experiment

- In the DIY-AI `1.0.0.163` ruleset, “生成” presents every eligible candidate
  and lets the player choose one. Do not reuse the legacy creation helper that
  shuffles and takes three. “己方起始牌组之外” excludes matching `CardId`
  values from the player's initial deck, and retired ordinary DIY cards remain
  unavailable even when they otherwise match a predicate.
- This rule applies to Whispering Hillock, Usurper, Princess Adda, Filavandrel,
  Aguara: True Form, Triss: Telekinesis, Kiyan, Isengrim: Outlaw, Hym, Black
  Blood, Garrison, Dorregaray, Vreemde, Mahakam Horn, Ornamental Sword, Uma's
  Curse, and all five faction runestones. Multi-mode cards keep their unrelated
  mode unchanged.
- Uma uses the highest current on-board power and the candidate's base power
  parity. On a tie, scan from top to bottom and left to right: enemy siege,
  enemy ranged, enemy melee, own melee, own ranged, own siege; within one row,
  use ascending card index. Those rows map respectively to Skellige,
  Scoia'tael, Northern Realms, Nilfgaard, Monsters, and Neutral. An empty board
  ends the effect. These tie and empty-board details are intentionally kept out
  of the compact player-facing card description.
- Faction runestones generate an even-power Bronze unit of their fixed faction
  while behind and an odd-power one while ahead: Devena is Monsters, Dazhbog
  is Nilfgaard, Zoria is Northern Realms, Morana is Scoia'tael, and Stribog is
  Skellige. A tied score ends the effect. Their candidates are non-spies
  outside the player's initial deck. Player-facing text must name that fixed
  faction explicitly instead of saying “your faction” or “this faction”.
- Triss: Telekinesis keeps its original source pool—Bronze special cards present
  in either player's initial deck—but presents all distinct eligible candidates.
