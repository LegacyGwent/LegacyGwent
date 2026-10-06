# October 7 card batch

Last verified: 2026-10-07

- CardMap 1.0.0.209 preserves all existing IDs and ordinals. Brehen 70209
  has 7 base power. Wraith Sorcerer 70194 and Ves 43002 change Chinese wording only.
- Egmond 70200 removes positive Boost and deals the same damage on Deploy,
  then repeats every two unlocked owner turn ends with a visible Countdown.
  Enemy turns, kills, and receiving Boost do not accelerate the timer. Deploy
  initializes a zero timer; replaying Deploy preserves an active timer.
- Gaetan 70208 snapshots other allied living units before its self-row damage.
  It gets three initial hits plus one per two snapshotted other units (floor).
  It recalculates opposing-row population before each hit and stops when that
  population no longer exceeds its current base Strength. Damage can select
  any otherwise legal unit on either half or another row.
- Brehen 70209 uses the same pre-damage other-unit snapshot. It no longer
  Strengthens. If its base Strength exceeds opposing-row population, every
  visible enemy on that row takes the difference plus half the snapshot (floor).
  The bonus does not independently enable damage when the comparison fails.
- Both population calculations include concealed Ambushes on either half;
  self-damage and enemy damage retain normal visible/living target filters.
  Snapshot counts remain fixed after lethal allied self-damage; Gaetan's enemy
  count remains live. Existing September 29 code omitted enemy Ambushes from
  the population filter; this batch completes that earlier explicit requirement.
- Card-ability exchange is not universally same-name-safe. Ves excludes its
  selected CardId and stops if only identical IDs remain. Shared Swap /
  GetDeckSwapCard do not impose this exclusion; Sarah, Elven Scout, Vrihedd
  Officer and War Council can still exchange into an identical card.
  Opening/round MulliganCard is separate: it blacklists already returned IDs
  while other eligible IDs remain, then falls back to that blacklist if needed.
- Verification uses production headless event/selection pipelines, including
  lock/owner timer gating, pre-damage lethal/hidden ally counts, Shield, and
  hidden enemy population versus damage legality. These are local tests, not
  live match or client rendering acceptance.
