# November 14 card and offboard state update

Last verified: 2026-11-14

- Dryad Matron (`70122`) strengthens other live visible same-row Dryads by 1,
  then selects one hand unit (including spies) to boost by the actual total
  base-strength increase. It does not distribute random boosts per Dryad.
- Brokilon Maiden (`70198`) generates one Doomed original copy on one selected
  other allied nonfull row. Both players' turn starts transform it to the
  original definition of the strongest visible living same-row Dryad with a
  different name. Current buffs, locks and damage are not copied. Ties retain
  the existing stable row order.
- Chinese card Info wording uses `变为` in place of `转化为`; internal Transform
  APIs retain their names.

## Offboard status publication

Symptom: Bone Talisman resurrection decremented cemetery Cerys on the server,
while the cemetery inspection still displayed the previous countdown.
Cause: movement published the cemetery before AfterCardResurrect decremented
Cerys; ShowSetCard only sent player updates for row/hand/stay cards. Number
animations do not update cemetery/deck CardStatus snapshots.
Fix: ShowSetCard publishes the existing full cemetery snapshot for cemetery
mutations and the existing owner-only deck snapshot for deck mutations. The
shared path covers countdown and other status changes; it does not leak an
opponent's private deck. Existing spectator visibility is retained.
Prevention: verify immutable serialized outgoing payloads, not just mutable
server CardStatus or an aliased in-process reference.
Verification: NovemberFourteenthTests checks cemetery countdown 4-to-3 on both
clients and private deck refresh without opponent deck payloads. Bone Talisman
executes its actual CardUseEffect through Game.AddTask. Cerys resets to 4 and
strengthens after its final tick, remains suppressed while locked, and waits at
zero with a full board until another successful resurrection provides a retry.
These are headless server/client-protocol checks, not Unity rendering evidence.
