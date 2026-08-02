# Business rules

Last verified: 2026-08-02

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
