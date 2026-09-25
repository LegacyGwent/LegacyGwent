# September 24 card batch

Last verified: 2026-09-25

## Identity and localization

- CardMap `1.0.0.203` has 736 entries. It preserves the first 728 entries and
  appends `70204` through `70211` in the order recorded in
  `card-pool-migrations.md`.
- The eight new cards are ordinary deckable cards. Add them to the exact Mongo
  reset allowlist and keep server, Unity Resources, and Unity StreamingFile
  locales aligned in Chinese, English, Polish, and Russian.
- Existing art `202801`, `202804`, `202805`, `202806`, and `202808` is already
  registered as full and slot Addressables for the five new Scoia'tael cards.
- Brokilon Sentinel `70015` and Giant's Belt `70171` also have registered full
  and slot art (`202273` and `203266`). Their September 24 effects did not make
  them deckable until they were removed from the retired manifest and added to
  the Mongo allowlist in CardMap `1.0.0.203`.

## Armor and repeat rules

- Bronibor lets its controller choose one allied row after Poor Infantry lands.
  Every Soldier on that row gains 1 Armor, and one enemy takes damage equal to
  the total Armor actually granted by that resolution.
- Damnation still locks and summons the two highest Bronze units. A summoned
  Bronze unit whose printed ability contains a standalone initial `X Armor`
  sentence receives that Armor after summoning. The verified IDs and values are
  `34004:2`, `34024:2`, `44001:4`, `44003:3`, `44006:4`, `44009:2`,
  `44010:2`, `44013:2`, `44024:1`, and `64022:2`. Do not execute the whole
  Deploy ability merely to restore printed Armor.
- Egmond removes only positive Boost from the chosen ally. Destroying the
  damage target schedules one repeat at the owner's turn end, as does Egmond
  receiving Boost during its owner's turn. A turn-end repeat cannot schedule
  another repeat.
- Lyrian Arbalest can target either side. It damages a lower-power target by the
  difference with normal Armor interaction; a target at least as strong loses
  all Armor and no power.

## New-card timing

- Vlodimir plays a random weakest Bronze/Silver Witcher from deck. While an
  unlocked allied Vlodimir remains on board, Gaunter O'Dimm gets up to three
  guesses; every failed guess rolls a new random unit, and a correct guess or
  the third failure ends the sequence.
- Gezras Strengthens the selected Bronze/Silver Witcher or Beast by 1, or
  Weakens its base Strength to 1, then clears temporary board state and replays
  that same card. The Strengthen/Weaken survives the replay.
- Gaetan and Cat School Witcher recalculate the current opposing-row population
  at their documented repeat or damage step. Cat School Witcher Thug moves the
  selected enemy to the opposing row first and recalculates that row before the
  repeated pair of hits. Brehen counts actual Armor/power lost by its allied
  one-damage hits before Strengthening itself.
