# September 15 card batch

Last verified: 2026-09-16

## CardMap and availability

- CardMap `1.0.0.199` has 728 entries. It preserves the previous 727 keys and
  appends Axel Three-Eyes `70203`; never renumber the historical prefix.
- Original Monsters Draug `22002` remains a persistent map slot but is retired
  from player decks. Northern Realms Draug `70197` is its deckable replacement.
  The Mongo migration replaces `22002` in decks and blacklists with `70197`, or
  removes the old duplicate if the replacement is already present.
- Crow `70136` is an active derived dependency, so it must not be retired or
  stripped from linked-card pools. `IsDerive` keeps it non-deckable and outside
  the Mongo allowlist.
- Axel is a 5-power Skellige Silver Druid. Its art `d19860000` is a local Unity
  Addressable without a separate `_slot`; the list-card fallback crops the
  readable full texture at an 8:1 rectangle centered on the character.

## Balance and metadata

- Miruna `22011` changes from Beast to Relict. Knickers `70110` changes 3 to 4,
  Protofleder `70010` changes 6 to 7, Immortal Cavalry `70101` changes 9 to 7,
  Crow Clan Druid `70134` changes 7 to 8, Van Moorlehem Hunter `70153` changes
  to 7, Philippe van Moorlehem `70151` changes 10 to 9, and Vincent van
  Moorlehem `70150` changes 7 to 6.
- Hunter selects only Bronze deck units with current power greater than 1;
  Philippe selects only deck units with current power greater than 1; Vincent
  inspects only non-Spying Bronze/Silver enemy deck units above 1 power. Their
  existing actual-power-lost damage and odd-half rounding behavior remains.

## Movement and Crow rules

- Gascon keeps the existing row-selection interaction, snapshots both players'
  corresponding rows, excludes himself, and moves every other unit to a random
  different non-full row. Each successful move removes at most one point of
  Gascon's current positive Boost. This is direct Boost loss: it does not deal
  damage, trigger damage listeners, reduce base Strength, or kill him. In hand
  or deck during his controller's turn, he gains 1 Boost when any other Bronze
  or Silver unit moves; Gold, self-movement, and enemy-turn movement do not
  trigger him.
- Crow Clan Druid creates one Crow immediately to its right. At each owner turn
  start, if its row contains a Crow, it repeats the creation at the right end
  when space permits and then damages itself by 1. An enemy turn or a row with
  no Crow does nothing.
- Crowmother has a visible Countdown starting at zero. While unlocked, it counts
  every Crow destroyed during the match, regardless of owner. On Deploy it
  creates one Crow in each other allied row, then creates the counted number in
  its own row until that row is full. It generates new Crow cards rather than
  resurrecting old cemetery instances.
- Axel Three-Eyes offers two localized choices: create one Crow in every allied
  row with space, or create three Crow's Eyes directly in the owner's cemetery.
  The second option must not play, discard, or resurrect those cards.
