# August 7 Northern Realms/global batch

Last verified: 2026-08-07

Load this reference when changing the August 7 first restoration batch or
diagnosing its delayed, armor, swap, or lock behavior.

## Persistent card-pool facts

- CardMap remains 718 entries in the same order at version `1.0.0.176`.
- Restore these 21 existing IDs to user decks without adding slots:
  `70017`, `70024`, `70033`, `70050`, `70076`, `70077`, `70078`, `70086`,
  `70094`, `70095`, `70101`, `70104`, `70118`, `70126`, `70130`, `70141`,
  `70142`, `70143`, `70144`, `70163`, `70188`.
- The exact Mongo allowlist and `DiyAiCardPool.IsUserDeckCard` must agree. The
  retirement manifest has 84 entries after this restoration.
- Keep server, Unity Resources, and Unity StreamingFile locales identical for
  Chinese, English, Polish, and Russian. Text changes require the same CardMap
  version bump even when IDs and ordering do not change.

## Non-obvious effect rules

- Cupbearer (`70152`) chooses only a revealed Bronze hand card. `Conceal` is a
  no-op on an unrevealed card, so selecting from all Bronze cards and Boosting
  afterward incorrectly grants a free Boost.
- Gael (`70146`) applies Golden Froth plus row-wide Drain on deploy, then repeats
  exactly once after the third owner turn start. Opponent starts do not count.
- Calanthe (`70179`) snapshots only positive Boost and current Armor, grants both
  directly to herself, repairs the chosen non-Spying Bronze/Silver ally, returns
  it to deck, then plays a Bronze/Silver unit. Do not implement the transfer via
  Damage or Drain: Shield, Armor, death, and damage events would alter it.
- Roach (`13001`) reacts to a Gold unit that enters through the Play pipeline,
  including a Gold played from deck. A pure Summon or Move must not trigger it.
- Lyrian Cavalry (`70094`) gains half the selected deck unit's positive Boost,
  rounded up; wounded or unboosted targets grant zero.
- Reynard Odo (`70163`) has no activation cap. Remove both runtime countdown
  state and CardMap countdown metadata.
- Mantlet (`70130`) gains 6 Armor once plus once per Crew trigger on deploy. Its
  Armor absorbs damage to either adjacent ally. Deduct Armor directly and emit
  Armor events; routing the deduction through self-Damage lets Shield prevent
  Armor loss after the ally's damage was already reduced.
- Immortal Cavalry (`70101`) starts by toggling itself locked. Locked cards are
  skipped by the production event dispatcher, so it cannot count turn starts or
  unlock itself until another effect unlocks it. Once unlocked on board, it
  toggles after two owner turn starts; opponent starts do not count.
- War Elephant (`70033`) destroys and totals its own and both adjacent allies'
  Armor before dealing that amount as damage. Its own Armor works even with no
  adjacent unit; direct Armor removal avoids Shield/damage-pipeline distortion.
- Ves performs two sequential single swaps. The second choice sees the hand
  produced by the first swap.
- Damned Sorceress deals 7 with one other Cursed ally and +1 for each additional
  same-row Cursed ally (`6 + count`).

## Regression anchors

- Static tests lock all 21 availability changes, representative strengths and
  categories, effect registrations, exact Chinese text, locale-root parity,
  CardMap version, map size/order, and Mongo allowlist equality.
- Headless tests must use `Game.SendEvent` for lock-silencing and normal damage
  routing, and deterministic one-card candidate pools for random/select effects.
- Required positive/negative anchors include unrevealed Cupbearer targets,
  Gael turns one/two/three, both Mantlet sides, locked/unlocked Immortal turns,
  odd/even Lyrian rounding, a fourth Reynard activation, and armor-only War
  Elephant with no neighbors.
