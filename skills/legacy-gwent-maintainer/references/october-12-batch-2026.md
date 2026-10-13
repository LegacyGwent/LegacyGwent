# October 12 card batch

Last verified: 2026-10-14

- CardMap `1.0.0.210` retains every ID and ordinal. The only restored deck card
  is Cultist Oblation `70090`, now named 斯瓦勃洛邪教徒: Bronze, 9 power,
  Cultist category, no Token or Countdown. Its retirement entry is removed and
  `deploy/diy-ai/reset-card-pool.js` includes it in the runtime-matching allowlist.
- Chinese card Info replaces 友方单位 with 友军单位. Ves `43002` uses the normal
  unrestricted Swap contract, including identical IDs; it supersedes the
  October 7 card-specific exclusion. Opening/round mulligans are unchanged.
- Immortal Cavalry `70101` rises from 7 to 8. Combat Engineer `34018` grants
  Resilience without Boost; Chinese Info is 使1个友军单位获得坚韧。操控。
- Reinforced Trebuchet `44008` copies the current DIY owner-turn-end damage:
  random enemy takes `1 + GetCrewedCount()`, without Strengthen. Odrin `43009`
  and Spectral Whale `65004` copy DIY turn and Deathwish effects; Whale remains
  a derived Token. Odrin wording starts 每回合开始时. Whale Deathwish damages
  its death row without moving, matching actual DIY behavior.
- These two copied Deathwish implementations correct DIY's shared off-by-one
  mistake: the filtered list already excludes the source, so an empty list
  alone stops the effect. A sole surviving unit receives the Boost/damage.
- Skjordal `70206` discards a Clan unit from the deck regardless of rarity,
  then resurrects a differently identified Bronze unit in the same Clan.
- Artis `70089` has 10 power, damages one allied unit by 9, and creates ONE
  Doomed Svalblod Fanatic `70046` in Artis's row with base power equal to actual
  power lost. Snapshot before Damage, clamp loss to `[0, before]`, and treat a
  target that left play as losing its snapshot power. This handles Shield,
  Armor, queued death, overkill and cemetery Repair. Zero loss creates a zero
  power unit through the ordinary creation/death pipeline; full-row creation
  follows ordinary capacity rules.
- Cultist listens to the common `AfterCardTransform` event while alive, unlocked
  and on board during its owner's turn. Any player's Unit becoming a different
  Unit triggers: create a Doomed same-name copy of SELF immediately to the left,
  copying SELF's current CardStatus, then deal SELF 4 damage. Creation does not
  transform, so it cannot recursively trigger this event. Subsequent real
  transformations also trigger surviving copies. New copies begin with fresh
  stateless effects; private effect internals are not copied.
- Transform captures old ID/type before replacing status; AfterCardTransform
  carries old type and old/new identities/types as immutable event facts.
  This prevents special-to-unit transformations (Artorius Vigo) and forced
  same-ID replacements from triggering Cultist and avoids deriving past facts
  from a mutable event Target. Coral, Artefact Compression, Raging Berserker
  and all other ordinary transform callers use this shared contract.
- `CardStatus.CopyCurrentStateFrom` copies gameplay/presentation fields and
  duplicates category/tag arrays while leaving the destination zone alone.
  Copying happens before CreateCard landing and before Cultist's self-damage;
  do not replace it with a pristine definition-only CreateCard or copy after
  damage. CreateCard preserves normal source-based premium propagation.
- Verification: `OctoberTwelfthBatchTests` exercises Armor/Shield/lethal actual
  loss, both sides' Coral/Compression/Berserker transformations, copy placement
  and pre-damage Strength/Boost/Armor/Resilience, owner-turn/Lock/same-ID gates,
  Vigo Special-to-Unit rejection, Engineer, Gold Clan discard and Bronze
  resurrection, and actual one-target Deathwish through AddTask -> Damage ->
  ToCemetery -> AfterCardDeath. Static tests cover metadata and pool parity.
  Local headless tests are not deployment or live/client acceptance.
