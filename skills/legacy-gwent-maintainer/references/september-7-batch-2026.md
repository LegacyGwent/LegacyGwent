# September 7, 2026 card batch

Last verified: 2026-09-07

- Original Draug `22002` restores the implementation immediately preceding
  September 2: random cemetery resurrection through match RNG until its row
  fills or the cemetery empties. No selection menu or new eight-card cap.
  Its existing description is unchanged.
- Northern Realms Draug `70197` now owns an independent effect rather than
  inheriting Draug. Offer only cemetery Units, allow zero/partial selection,
  cap selections at eight and current free row space, then transform each
  selected instance to a 1-power Draugir and resurrect it into the source row.
  Art, faction, power and linked-card metadata are unchanged.
- Calanthe `70179` still keeps its Bronze/Silver Unit restriction when playing
  from deck. Exclude both runtime IsSpying and intrinsic enemy-side placement
  (EnemyRow/EnemyPlace). Do not change her existing transfer/repair phase.
  Symptom: False Ciri was offered despite the non-spy text. Cause: deck spies
  normally have IsSpying=false until played. Fix: inspect CardUseInfo too.
  Prevention/verification: use a real unplayed False Ciri, not only a Wolf with
  a manually set spy flag; assert the actual menu and the normal unit's play.
- Mad Charge `70050` accepts an ally with negative HealthStatus OR positive
  Armor. A healthy, unarmored ally and a positively boosted, unarmored ally
  are ineligible. Duel behavior is otherwise unchanged.
- Feast of Blood `70183` has only a Chinese reminder-text addition, quoting
  Plumard's exact current rule. Its effect is unchanged. The old global ban on
  “汲取” now permits Plumard and its quoted reminder in Feast of Blood; the
  terminal-punctuation check accepts a closing full-width parenthesis after
  sentence punctuation. Exact new text is asserted across all three CN files.
- Regression coverage: SeptemberSeventhFirstBatchTests and
  DiyAiCardPoolTests.SeptemberSeventhFirstBatchMatchesRulesMetadataAndChineseLocales.
  The card-map version is `1.0.0.195`; historical card order stays unchanged.
