# September 29 card batch

Last verified: 2026-09-29

- CardMap `1.0.0.207` keeps all 737 existing IDs and ordinals. Ophelie Van
  Moorlehem `70205` has 5 base power; its canonical Chinese name remains
  `欧菲丽·凡·莫拉汉姆`.
- Gaetan `70208` snapshots the number of **other** living units on its own
  row before dealing 1 damage to visible allied units. A face-down Ambush is
  included in the snapshot but is not damaged. It performs one initial hit plus
  one repetition per snapshotted other unit. Later allied deaths do not reduce
  this count. Its target remains selectable from any row, and opposing-row
  population is recalculated before each hit.
- Brehen `70209` snapshots the number of living units on its own row,
  **including Brehen**, before damaging other visible allies. Face-down
  Ambushes contribute to this count but are not damaged. It Strengthens itself
  by half the snapshot count, rounded down by integer division, regardless of
  damage prevented by Shield or Armor or subsequent deaths. The strengthened
  base power is used in its opposing-row difference.
- Van Moorlehem Servant `70127` retains Deploy Immunity. When Concealed from
  hand during its controller's turn, it gains 4 Boost and Reveals itself. This
  is governed by `GameRound`, not by the Conceal effect's source controller.
  On the opponent's turn it stays Concealed and gains no Boost.
- The three locale surfaces (server, Unity Resources, Unity StreamingFile)
  carry matching Chinese, English, Polish, and Russian text for Brehen and the
  servant. The map's Chinese `Info` matches server Chinese.
