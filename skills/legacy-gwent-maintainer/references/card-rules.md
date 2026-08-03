# Card-specific rules

Last verified: 2026-08-03

## Similar Chinese card names

- `CardId.DimunPirate` (`64002`) is “迪门家族海盗”: base strength 11 and
  discards all remaining copies of itself from the deck.
- `CardId.DimunCorsair` (`64028`) is “迪门家族海贼”: base strength 1 after the
  2026-08-02 balance patch and resurrects a Bronze Machine unit.
- Do not identify these cards from the shared “迪门家族海…” prefix. Confirm the
  exact Chinese name, `CardId`, art, and effect text before applying balance data.

## Selected DIY cards in the reset pool

- `70001` is 昆恩法印 and is a deckable Copper Neutral spell in DIY-AI
  `1.0.0.165`. It immediately Boosts the selected Bronze/Silver hand unit and
  all same-ID cards currently in hand/deck by 2, then gives each an ordinary
  Shield. A revealed unit in hand therefore blocks one damage instance.
- The shared Duel rule clears the initiating unit's Shield before its first
  attack; the target's Shield still blocks the first incoming attack normally.
  The Shield is not restored. In forced two-unit Duel effects such as Treason,
  the first selected unit is the initiator. This makes two Shielded units
  resolve without a special origin flag or a non-progressing loop.
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

## August 3 public test batch

- DIY-AI `1.0.0.165` re-enables these deckable DIY cards: `70002`, `70005`,
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
