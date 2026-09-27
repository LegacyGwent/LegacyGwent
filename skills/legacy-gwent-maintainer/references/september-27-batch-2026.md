# September 27 card batch

Last verified: 2026-09-27

## CardMap and identity

- CardMap `1.0.0.206` has 737 ordered entries. Raffard's Vengeance `70212`
  (拉法达的复仇) is appended after `70211`; preserve all prior ordinals.
  It is a deckable Northern Realms Silver Machine with 9 base power, visible
  Countdown 4, and CardMap art ID `203050`. The art ID is metadata; full-art
  and miniature availability require separate client-asset verification.
- The September 27 CardMap art mappings are Rumourmonger `70195:202319`,
  Endrega Queen `70199:202438`, Egmond `70200:202648`, Ramon Tyrconnel
  `70202:202446`, Axel Three-Eyes `70203:202516`, and new Raffard's Vengeance
  `70212:203050`. Dana Meadbh `70191` already uses `203195` and is unchanged.
- Corrupted Flaminca `70013` changes its second category from Support to
  Cultist while retaining Clan Heymaey, card identity, and rain effect. Keep
  existing `CardId.CorruptedFlaminca` and `CardId.AxelThreeEyes` names; art
  and category changes do not allocate new IDs or rename effect classes.

## Raffard's Vengeance timing

- `RaffardsVengeance.cs` uses the shared `CardStatus.Countdown` and
  `IsCountdown`, not effect-instance fields. This matters because the game
  dispatches turn events through `Card.Effects`, while Siege Master `44019`
  directly invokes the target's primary `Card.Effect.CardPlayEffect`.
- On Deploy, Countdown begins at 4 and each unlocked adjacent Crew count
  reduces it once, in order. Two Crew leave it at 2. Siege Master repeats
  this Crew reduction against the current Countdown; it does not reinitialize
  the counter.
- On each **owner** turn end, decrement once. If that does not fire, do not
  apply Crew again: a counter of 2 becomes 1 even with Crew on both sides.
  When the decrement fires, select one battlefield unit on either side (the
  unqualified damage text does not require an enemy), deal 10 damage plus 1 for
  each visible, living Mage in Raffard's physical row (including enemy-owned
  Spying units there), then reset Countdown to
  4. With no legal target, reset after the empty-target resolution. Only then
  apply the current adjacent Crew steps. Thus two Crew leave the post-shot
  counter at 2.
- A Crew decrement that itself fires also finishes damage before resetting to
  4. Continue only the remaining already-counted Crew steps; do not
  recursively reapply Crew after that reset. With two Crew and Countdown 1,
  Siege Master's replay fires on the first Crew step and leaves Countdown 3
  after the second. Damage events observe Countdown 0 before reset. Locking
  Raffard silences owner-turn triggers and Deploy replay; locked Crew does
  not contribute reductions.
- `SeptemberTwentySeventhRaffardsVengeanceTests.cs` covers owner/other-turn
  cadence, deploy and post-shot two-Crew states, Siege Master's real replay,
  same-row Mage damage including an enemy-owned spy, the damage-event
  counter observation, lock, unrestricted allied targeting, and no-target
  reset. The isolated gameplay suite passes 301 tests, including five focused
  Raffard scenarios.

## Artwork import and delivery

- The six existing full sprites for art IDs `203195`, `202438`, `202446`,
  `202648`, `202516`, and `202319` already have Unity `.meta` GUIDs and
  Addressables entries. The checked-in `203050_slot` miniature likewise has
  its own GUID and Miniatures entry, but no historical `203050` full sprite
  was found in repository history. The new full sprite is therefore a
  documented import from the public [gwent.one card page](https://gwent.one/en/card/203050),
  not a restoration of an exact historical Unity asset.
- Source: `https://gwent.one/image/gwent/assets/card/art/max/3390.png`,
  992×1424 PNG, SHA-256
  `ba11a1cad38b2bce05a3a75768e0a1be52c78982f57b58de60bacbd91a96b9ba`.
  Its picture matches the existing `203050_slot` miniature. Import by
  bicubic downsampling to 496×712 pixels and placing it at the upper-left
  of an opaque black 1024×1024 RGBA canvas, matching the established Unity
  full-sprite layout. This is downsampling; no source pixels are upscaled.
  `203050.png.meta` uses the same Unity 2019 sprite importer settings as
  neighboring full cards with its own GUID; `Default Local Group.asset`
  registers that GUID at address `203050`.
- Server `wwwroot/scale` now has 120×173 previews for all seven art IDs.
  Each is bicubically reduced from the 496×712 picture rectangle of its
  corresponding Unity full sprite, rather than from black canvas padding.
  A client built before this import still needs a rebuild to show the new
  `203050` full image; a server CardMap sync cannot install local artwork.
