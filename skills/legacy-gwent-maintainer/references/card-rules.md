# Card-specific rules

Last verified: 2026-08-23

## August 23 first card batch

- Ves (`43002`) performs up to two hand/deck swaps. For each swap, both the
  selectable hand card and the randomly chosen deck replacement must have at
  least one counterpart with a different `CardId`; a card can never be swapped
  for an original same-name copy.
- Cerys: Fearless (`62010`) is a 6-power unit. She first Duels a selected enemy;
  only if she remains on the battlefield afterward does the currently strongest
  allied Drummond Queensguard Duel a second selected enemy. Do not present an
  allied-Queensguard selection: ties follow the existing stable battlefield
  order. Her former discard/resurrection counter behavior is retired.
- Wraith Sorcerer (`70194`) maps to full art `c10002300` and miniature
  `c10002300_slot`; both addresses are local Unity Addressables. Server CardMap
  and website-scale updates do not add them to an already installed client, so
  an older client may remain blank until a client containing the rebuilt local
  Addressables catalog/bundles is installed.
- Ulle the Unlucky (`70178`) keeps the existing effect implementation. Its text
  says “if it survives” rather than “if it wins”, matching the actual post-Duel
  survival check.

## Similar Chinese card names

- `CardId.DimunPirate` (`64002`) is “迪门家族海盗”: base strength 11 and
  discards all remaining copies of itself from the deck.
- `CardId.DimunCorsair` (`64028`) is “迪门家族海贼”: base strength 1 after the
  2026-08-02 balance patch and resurrects a Bronze Machine unit.
- Do not identify these cards from the shared “迪门家族海…” prefix. Confirm the
  exact Chinese name, `CardId`, art, and effect text before applying balance data.

## Selected DIY cards in the reset pool

- `70001` is 昆恩法印 and is a deckable Copper Neutral spell in DIY-AI
  `1.0.0.168`. It immediately Boosts the selected Bronze/Silver hand unit and
  all same-ID cards currently in hand/deck by 2, then gives each an ordinary
  Shield. A revealed unit in hand therefore blocks one damage instance.
- Duel has no Quen-specific or initiator-specific shield exception. Both units'
  Shields block their first incoming damage instance through the normal damage
  pipeline; when both start Shielded, each Shield is consumed in the first
  exchange and the Duel then progresses normally.
- Ice Troll (`24034`) is the card-specific balance valve: it has 5 base power
  and deals 1 damage to itself before selecting and starting its Duel. That
  self-damage naturally consumes its Shield if present. If the target row has
  Biting Frost, only Ice Troll's Duel damage uses the x2 multiplier.
- `70041` is 鬼针草煎药 and `70042` is 合欢茎魔药. DIY-AI deliberately
  makes both cards deckable while the rest of the ordinary DIY retirement
  manifest stays hidden.
- Use exactly `Special + Alchemy`; neither card has the `Item` category. This
  lets alchemy effects such as Viper Witcher count them without making them
  eligible for item-only effects.
- Both effects apply 2 points once and then repeat that application three times,
  with no separate initial 3-point hit/boost. Each copy of the counterpart
  potion in the player's graveyard adds one more 2-point repetition. Player text
  must say “随后重复3次” rather than the ambiguous “重复4次”.

## Retired temporary balance variants

- Since `1.0.0.168`, only original Viper Witcher `34022` and original An Craite
  Greatsword `64009` are user-deck cards. A/B/C IDs `34034`-`34036` and
  `64035`-`64037` are retired, hidden, rejected by deck validation, removed from
  Mongo decks/blacklists, and must not be offered to players.
- Keep those six IDs as invisible historical CardMap slots, with their metadata,
  locale entries, and effect types dormant. Never delete or reuse them: they
  occupied published ordinals 709-714, and the next new card is appended after
  them. “Remove the variants” means player-facing retirement, not ordinal reuse.
- Living Armor is the intended protection experiment for original engines. Its
  regression test keeps the original Greatsword alive through 9 incoming damage
  (halved to 5), then verifies its two-turn heal/Strengthen-2 cycle.

## Saesenthessis: Blaze refill

- `12006` banishes the other cards remaining in its controller's hand, then
  draws that same count one card at a time.
- When the deck is empty before a required draw, snapshot every non-Leader,
  non-Spying unit currently in that player's cemetery and return each
  still-eligible unit to a random deck position through the normal
  `Resurrect(... MyDeck ...)` helper. Leaders, Spying units, and non-unit cards
  remain in the cemetery. Continue drawing afterward; stop cleanly if both the
  deck and eligible cemetery-unit pool are empty. In code, non-Spying follows
  the established hand/deck convention `CardUseInfo == MyRow`.
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

## August 3 public test batch

- DIY-AI `1.0.0.167` re-enables these deckable DIY cards: `70002`, `70005`,
  `70011`, `70026`, `70027`, `70059`, `70062`, `70070`, `70091`, `70110`,
  `70119`, `70131`, `70133`, `70155`, `70157`, `70161`, `70172`, and `70190`.
  Their derived dependencies `70006`, `70071`, and `70162` are available only
  as generated/transformed cards, never as user-deck cards. Keep the runtime
  pool and Mongo migration allowlist identical.
- Living Armor has no deploy Armor. One unlocked Living Armor per allied row
  halves each damage instance to any allied unit on that row, rounded up;
  multiple copies never stack. Ivo of Belhaven's Deathwish runs only when Ivo
  himself dies, not whenever any unit dies.
- Lady of the Lake weakens by the remaining hand/deck card count once. Thaw
  applies four base 2-point boosts (initial application plus three repeats),
  then one more for each card played earlier that turn; `TurnCardPlayedNum`
  already includes the current Thaw during its deploy effect.
- Lonely Champion checks its row and the whole allied board independently at
  owner turn end. Princess transforms one Bear on its row at every owner turn
  start. Aguara's hand option excludes spies and every menu option must retain
  its own localization key. Old Speartip uses two complete forms; Magic Lamp in
  the cemetery makes The Last Wish inspect one additional card.

## August 4 leader batch

- `1.0.0.168` restores the existing DIY definitions of Meve `70045` and Anna
  Henrietta `70149`, restores/reworks Queen Calanthe `70179`, and appends Dana
  Meadbh `70191`. All four are user-deck leaders; Calanthe remains 7 power,
  Meve 8, Anna 6, and Dana 3.
- Since the `1.0.0.169` clarification, Calanthe snapshots only one allied
  non-Spying Bronze/Silver unit's current positive net Boost, resets that Boost,
  gains the same amount as Boost, shuffles the unit into the deck, then forces
  the player to choose and play a Bronze/Silver unit from the deck. Do not
  consume Armor, and do not call generic `Drain` or `Damage`: Shield does not
  block the direct Boost transfer, while Armor, Shield, Resilience, negative
  `HealthStatus`, and other unmentioned state remain untouched.
- Meve and Anna source files already matched `origin/diy`; their reset-state bug
  was availability, not missing behavior. Meve Boosts one unit in board/hand/deck
  by 4. Anna sorts by base Strength and plays the lowest top card, including a
  Special when it is the lowest.
- Dana presents every Neutral deck card without unit or rarity restrictions,
  moves the chosen card to Stay, and plays it through the normal pipeline. The
  high-value chain Dana -> Royal Decree -> any Gold unit is intentional test
  coverage. A Gold unit played from the deck this way must trigger Roach.
  `AfterUnitDown.IsPlayed` marks entry through `CardEffect.Play` independently
  of `IsFromHand`; Summon, Resurrect, and Move leave it false and must not
  trigger Roach.
- Dana currently uses existing client art `203195`, including its registered
  full sprite and leader miniature, so the August 4 release can hot-sync without an
  immediate client rebuild. The originally proposed `d17210000` remains a
  future full-art restoration candidate. Server `wwwroot/scale` is only a small
  website image and is not a Unity asset source.

## August 4 second card batch

- CardMap `1.0.0.171` restores these user-deck cards from DIY: Prophet Lebioda
  `70007`, Vivienne: Oriole `70008`, Gascon `70032`, Radeyah `70072`, Barnabas
  Beckenbauer `70125`, Moon Dust `70128`, Piercing Missile `70156`, and Albastra
  `70180`; it also reworks Syanna `70025` and Coën of Poviss `70158`. Albastra's
  wings `70181`/`70182` are active derived dependencies, never deckable. Keep
  all ten deckable IDs aligned with the Mongo migration allowlist.
- Gascon snapshots the selected row and excludes himself before moving units;
  this prevents both self-movement and collection mutation during iteration.
  Its player-facing text intentionally does not mention the self-exclusion.
- Albastra applies Biting Frost to the enemy row opposite her current row on
  Deploy, then repeats only that Frost effect every second owner turn start via
  the ordinary visible Countdown. Her existing wing-presence destruction rule
  remains. Coën boosts every tied weakest other allied Witcher at owner turn
  start. For his Deathwish, pass the captured death row unchanged to
  `CreateCard(Farmer, AnotherPlayer, ...)`; `CreateCard` interprets that
  `MyRowN` relative to the destination player, producing the physical opposite
  row without calling `Mirror()`.

For the August 5 and August 6 monster batches, read
[monster-batches-august-2026.md](monster-batches-august-2026.md).

For the August 30 batch, read
[august-30-batch-2026.md](august-30-batch-2026.md).
