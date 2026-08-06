# August 2026 monster card batches

Last verified: 2026-08-06

Load this reference for the August 5 or August 6 DIY-AI Monster changes.

## August 5 monster batch

- CardMap `1.0.0.172` restores these DIY cards to the user pool: Olgierd:
  Immortal `70084`, Dettlaff: Crimson Curse `70102`, Keltullis `70113`, Orianna
  `70145`, Iris: Shade `70154`, Tatterwing `70164`, Cloud Giant `70170`, and Sir
  Scratch-a-Lot `70177`. Old Speartip: Asleep `22001` remains deckable, while
  `22003` is renamed Old Speartip: Awakened and becomes derived-only. The reset
  migration allowlist must exclude `22003` and include all eight restored IDs.
- Geralt: Aard keeps 6 power and uses the DIY siege-row branch: a target already
  on the Siege row takes 2 extra damage instead of moving upward, then receives
  the ordinary 3 damage. Geralt: Professional directly Banishes a Monster, so
  neither cemetery-entry nor Deathwish events fire. Imlerith destroys a target
  under Biting Frost instead of dealing 8 damage.
- Sir Scratch-a-Lot Boosts every Beast under Full Moon on either half by 1 on
  Deploy. Every individual copy in hand, deck, or the allied half independently
  listens to `AfterUnitDown` and Boosts only itself by 1 when an allied Beast
  enters play. This intentionally includes ordinary plays, Summon, and generated
  units such as Harpy Hatchlings and Woodland Spirit's Wolves; movement does not
  broadcast `AfterUnitDown`. Never make one listener loop over all copies, or N
  copies will produce N-squared Boosts.
- Cloud Giant had 7 power and Resilience in the August 5 build. On Deploy it
  counts Impenetrable Fog rows on the opposing half and gains that many owner-
  turn intervals of Immunity. Decrement at each subsequent owner turn start;
  one Fog therefore protects it through the opponent's next action window. The
  serialized `IsImmue` state drives selection rules, and the client uses a cool-
  blue card tint so the temporary state is visible without a new asset.
- Keltullis has 9 power and gains 2 Armor on Deploy. At owner turn end it first
  destroys one random tied-weakest other non-Immune ally on its row. Only after
  a valid sacrifice does it Boost itself by 1 and destroy one random tied-
  weakest non-Immune enemy below its new power opposite; no target ends both
  follow-ups.
- Tatterwing snapshots all opposite-row enemies before moving them, avoiding
  collection mutation. Dettlaff accepts Gold and Leader Beasts/Vampires too.

## August 6 monster batch

- CardMap `1.0.0.173` appends Ignis Fatuus `70192` and restores these user-deck
  IDs: `70009`, `70010`, `70022`, `70023`, `70058`, `70083`, `70085`, `70088`,
  `70106`, `70124`, `70129`, `70132`, `70146`, `70148`, `70168`, `70169`,
  `70176`, `70183`, and `70185`. The dependencies `70107`, `70108`, `70147`,
  `70186`, and `70187` stay derived-only and non-deckable. Keep all 20 deckable
  IDs and five derived dependencies aligned with the exact Mongo allowlist.
- Red Rider is a 6-power Silver and listens from the deck. It summons after the
  third enemy unit is destroyed from a Biting Frost row; round-end cemetery
  cleanup does not count. Apiarian Phantom deals 6 and applies Biting Frost
  only when the target has actually left the battlefield—checking `IsDead`
  after `Damage` is unreliable because cemetery movement repairs card state.
- Gael applies Golden Froth to the opposite row, Drains every unit there by 2,
  and repeats exactly once at the next owner turn start. Plumard is derived-only
  and only echoes a different same-row ally's Drain for 1; its old self-summon
  listeners are removed. Ice Giant uses the DIY Frost engine: Boost 3 per
  existing Biting Frost on Deploy and per later Frost application.
- Water Hag (`70169`) selects up to three other allies: ordinary targets gain a
  1-point Boost while Ogroids gain 1 Strength. Ogre Warrior (`70168`) checks
  once at the third subsequent owner turn start; if its side controls or ties
  the highest unit, it Boosts itself by half its current power rounded up.
  Hybrid triggers one unlocked allied Bronze unit's Deathwish without killing
  it, then performs a fresh allied-unit selection to Consume.
- Cloud Giant is now 8 power without Resilience. Fog rows grant one owner-turn
  interval of Immunity each. At every owner turn end it damages a random tied-
  highest enemy opposite by half its current base Strength rounded up; a
  survivor moves to a different random non-full row.
- Ignis Fatuus is a 6-power Gold Relict using existing art `202680`. Deploy
  creates one Doomed base copy on its row without recursively deploying it.
  While any unlocked copy remains on the allied battlefield, enemy
  Impenetrable Fog damage becomes Weaken. Multiple copies are idempotent, and
  the conversion occurs before Shield so the Shield remains available.
