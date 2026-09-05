# September 5, 2026 card batch

Last verified: 2026-09-05

- Alp `70148` resolves at owner turn end while alive on the board. Compare
  current total power against the strongest visible, living enemy in the
  physical opposite row. If none is stronger, drain exactly one strongest
  unit for 1; break strongest ties using the match RNG. Do not perform a
  second random selection from the whole row after checking its maximum.
- The Thing in the Swamp `70088` snapshots every friendly cemetery Foglet
  `24028`, repairs and moves all of them to the deck bottom in order, then
  lets its owner choose an enemy row for Impenetrable Fog. It repeats the
  entire effect when Torrential Rain is applied to the enemy half during
  its owner's turn, while alive on board. Use `AfterWeatherApply`, not
  weather damage or turn ticks; its own Fog must not recurse. Ordinary
  event dispatch supplies lock suppression. Returned Foglets can respond
  to the subsequent Fog normally.
- Milaen `53014` retains enemy-row selection. Snapshot damage as 6 plus the count
  of living friendly face-down Ambush units anywhere on the battlefield.
  Include concealed cards explicitly in the query, then require Unit type,
  Ambush category, and Conceal. Revealed Ambush units, opponent Ambushes,
  hand/deck cards, and concealed non-Ambush cards do not count. Resolve both
  physical endpoints, but a one-unit row is damaged only once.
- Sihil `12042`'s third option offers all Bronze/Silver units in the owner's
  deck. Move the chosen instance to Stay and return the normal follow-up
  play count; it is no longer random. Its odd/even three-damage options
  are unchanged.
- Chinese wording-only changes cover Iorveth: Meditation `52012`, Artis
  `70089`, Svalblod `70099`, Cloud Giant `70170`, and Knickers `70110`.
  Weavess: Incantation `22012`'s base description has no stray space inside
  the Relict-card phrase. Preserve the historical `_1_Strenghten` locale-key
  spelling; both choice descriptions survive JSON round-trip distinctly.
  This is payload verification, not proof of Unity visual layout.
- Menu text has its own versioned data: Vandergrift's Blade's damage
  option must say 10, not the old 9, and Sihil's third option must not
  say random. Keep these cn/en/pl/ru keys synchronized across all three
  locale surfaces; source-effect tests alone cannot detect stale options.
- Verification lives in `SeptemberFifthFirstBatchTests` and
  `DiyAiCardPoolTests.SeptemberFifthFirstBatchMatchesPublishedRulesAndChineseLocales`.
  The fixture starts with a populated basic deck: clear only the
  relevant test deck before asserting an exact selection pool. Inspect
  the deck at the row-selection boundary to prove bottom placement
  before Foglet's weather-triggered summon changes it.
