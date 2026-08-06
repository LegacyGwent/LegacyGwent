# August 6 Nilfgaard and global card batch

Last verified: 2026-08-06

Load this reference when changing the August 6 Nilfgaard restoration, global
half-value arithmetic, or its regression tests.

## Persistent rules

- All card-effect calculations described as half use ceiling division for
  positive integers: `(value + 1) / 2`. Card descriptions do not repeat an
  explicit rounding annotation.
- CardMap `1.0.0.174` contains 718 ordered entries. `70193` is appended; never
  remove or reuse it.
- Restored deckable DIY IDs are `70004`, `70012`, `70103`, `70111`, `70115`,
  `70123`, `70127`, `70150`, `70151`, `70152`, `70153`, `70165`, `70174`, and
  `70184`.
- Masquerade (`70193`) is a Nilfgaard Bronze Tactic using art `d19930000`. It
  targets a non-Leader unit and lets the player change its runtime rarity to one
  of the other two Gold/Silver/Bronze groups.

## Effect details worth preserving

- Garrison's base-deck generation pool is Bronze/Silver, Unit, non-Agent, and
  either Soldier or Officer.
- Assassination makes two separate penetrating 8-damage selections.
- Treason may select any two other enemy units in the same row; adjacency is not
  required.
- Hefty Helge normally damages enemy units not on its corresponding row. When
  revealed, it damages every enemy battlefield unit plus revealed non-Spying
  enemy Unit cards in hand and deck.
- Nilfgaardian Knight reveals the leftmost lowest-rarity own hand card and gains
  2 Armor. Ties are deterministic, not random.
- Cupbearer conceals one random Bronze own-hand card and Boosts it by 1 at every
  owner turn start.
- Mage Infiltrator copies an eligible enemy battlefield unit or revealed enemy
  hand unit to the opponent's board as a Doomed base copy; no eligible target
  ends cleanly.
- Alba Pikeman is the DIY behavior, not the old implementation: it gains 2 Armor
  on deploy and summons one same-ID deck copy at each owner turn start. A prior
  change updated only its description while leaving the old summon-all code;
  always test behavior as well as map text when restoring from DIY.
- Hybrid's Deathwish arms a one-shot flag; at the next owner turn start it
  consumes the immediate right unit. It does not consume on the opponent turn.
- Xarthisius moves the selected enemy deck card to the bottom and toggles Lock.

## Ignis Fatuus compatibility

Ignis only rewrites `DamageType.ImpenetrableFog` damage into Weaken. Apiarian
Phantom deals ordinary Unit damage, so the two effects must remain independent.
For Phantom's exact-6 hit, determine lethality by whether the target remains on
the battlefield after damage, not by `GameCard.IsDead`: cemetery movement calls
repair/reset before the awaiting effect resumes. The required matrix is:

- Ignis + lethal: exactly one Frost.
- Ignis + nonlethal: no Frost.
- no Ignis + lethal: exactly one Frost.
- no Ignis + nonlethal: no Frost.

## Verification

- `AugustSixthNilfgaardBatchTests` contains the dynamic behavior suite.
- `AugustSixthMonsterBatchTests` contains the Ignis/Apiarian four-quadrant test.
- `DiyAiCardPoolTests.AugustSixthNilfgaardBatchMatchesRulesPoolAndLocales`
  locks map, availability, locale copies, source arithmetic, and forbidden text.
