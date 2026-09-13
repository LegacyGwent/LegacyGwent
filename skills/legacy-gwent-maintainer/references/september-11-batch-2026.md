# September 11, 2026 card batch

Last verified: 2026-09-13

- CardMap `1.0.0.196` preserves all historical card slots. Treant Boar `70139`
  changes Chinese text only; its existing repeat implementation is unchanged.
  Dwarf Berserker `70109` orders categories Soldier, Dwarf. Filavandrel `51003`
  has 3 power; Dana Meadbh `70191` has 2 power and categories Leader, Relict.
- Vandergrift's Blade `43021` no longer presents a choose-one menu. Select one
  legal battlefield target, deal initially 10 ordinary damage, then destroy a
  surviving Bronze/Silver Cursed target. Gold Cursed targets take only damage.
  As in the previous damage branch, every target killed by this ability is
  banished before Deathwish. Mark Doomed before Damage so queued cemetery
  handling cannot race the marker; restore the prior value and client state
  for a survivor. Shield/Armor block ordinary damage but not the subsequent
  Bronze/Silver Cursed destruction.
- Blade handles `AfterRoundOver`, which the server sends before board cleanup.
  Return only if that physical Blade is already in its cemetery at that event;
  insert into its owner's deck using match RNG and add 3 to its damage. Each
  effect instance retains its own 10/13/16 progression through cemetery Repair.
  Production `CardUse` and round events both dispatch through `Card.Effects`;
  do not mix direct primary `Card.Effect.CardUseEffect()` with that event state.
- Sigvald `70038` attempts resurrection at every owner turn end while in the
  cemetery. Check for a legal free-row location before Resurrect. The visible
  initial Countdown of 2 now counts successful self resurrection events, not
  turns. `AfterCardResurrect` also counts external resurrection of this exact
  physical card. On every second success, reset to 2 and Strengthen by 1 only
  when current `CardPoint()` is at most 7; 7 can become 8. Failed attempts,
  standing on the board, and resurrection of another card do not consume it.
- Dandelion: Vainglory `12011` selects exactly one hand card with ID `12004`
  (Geralt of Rivia) or `12007` (Triss Merigold). Character HideTags and other
  variants are ineligible. Its old starting-deck Boost is removed. Use normal
  hand Play and queue the subsequent draw after that card's deployment. With
  no eligible hand card or no legal board space, do not draw; this follows the
  existing hand-play-then-draw eligibility rule of Sile and Battle Preparation.
  An empty deck does not prevent playing the selected hand card.

## Nested hand play on a full board

- Symptom: a nested AI `GetPlayCard` can throw on an empty location list.
- Cause: `RandomAutoAIPlayer.PlayCard` takes its first legal location directly;
  the outer normal-turn random-play guard does not protect nested calls.
- Fix: Dandelion checks whether all allied rows are full before requesting the
  nested hand play. No shared AI or rules/replay code changes are needed.
- Prevention: test full-board/no-target cases alongside successful deployment,
  and assert deployment effects cannot see the later replacement draw.
- Verification: `SeptemberEleventhSigvaldDandelionTests` exercises the real
  task pipeline, exact card selection, full board, and Triss-before-draw order.
  `SeptemberEleventhBladeTests` verifies real CardUse and round-event cycles,
  including 10/13/16 damage, multiple copies, shields, and cemetery gating.
  `DiyAiCardPoolTests.SeptemberEleventhFirstBatchMatchesRulesMetadataAndLocales`
  checks all seven cards and all three locale surfaces.
