# Gameplay lifecycle

Last verified: 2026-08-03

Use this reference before changing deployment, landing, shields, duels, or any
effect that must run at a precise point in a unit's entry pipeline. Verify the
relevant source again when changing the shared pipeline.

## Normal unit play

`CardEffects/CardEffect.cs::Play` expresses this order:

1. Send `BeforeUnitPlay`.
2. `CardPlayStart` moves the unit to its chosen row and clears reveal state.
3. Send `AfterUnitPlay`.
4. Raise `CardPlayEffect`; ordinary deployment effects execute here.
5. If the unit remains on a row, call `CardDown`: show the landed card, update
   points, resolve spying, then send `AfterUnitDown` and record history.
6. Resolve `BeforePlayStayCard` and any cards waiting in `PlayersStay`.
7. If the original unit remains on a row, raise `CardDownEffect`.

A unit removed during `CardPlayEffect` never reaches `CardDown` or
`CardDownEffect`. A concealed unit exits immediately after `ShowCardDown` and
does not follow the ordinary event sequence. Summon, resurrect, move, and manual
placement helpers may call different subsets; inspect their caller instead of
assuming they are equivalent to a hand play.

## Landing reactions

`CardDown` calls `ShowCardDown` before `AfterUnitDown`. Landing listeners can
therefore change or damage the unit before `CardDownEffect`. Active examples
include Savage Bear damage, Blood Moon damage, Pit Trap damage, Roach summons,
and several engines that boost themselves or the arriving unit.

If a new state must not affect deployment but must protect against landing
reactions, activate it after `ShowCardDown` and before `AfterUnitDown`. Putting
it only in `CardDownEffect` is too late. The current shared pipeline has no
named hook at that boundary, so add an explicit one-shot transition rather than
depending on animation timing.

## Damage and shield

`Damage` sends `BeforeCardDamage`, adopts any redirected target/value/type, and
then checks `Card.Status.IsShield`. A shield clears itself and cancels the whole
damage instance before armor or lethal handling. This applies to every caller
of `Damage`, including duel, weather, trap, and hand damage; there is no native
duel-only shield exception.

`IsShield` is serialized client-visible state and is valid in hand or on the
board. `ToCemetery`, `Repair`, and locking a card clear it. A future delayed
shield marker should be server-internal, one-shot, cleared when its card leaves
the eligible lifecycle, and converted to `IsShield` only at its specified
timing boundary.

## Duel and repeated deployment

The shared `Duel` helper deals the source's current power to the target, then—if
the target survives—deals the target's current power back, repeating until one
side leaves play or the safety limit is reached. Custom duel-like effects such
as Ice Troll's frost branch reproduce this loop and must be audited separately
when changing global duel behavior.

Syanna records both `AfterUnitPlay` and `AfterUnitDown`, then directly repeats
the target's `CardPlayEffect` and `CardDownEffect` at `AfterRoundPlay`. Do not put
one-shot state removal/restoration solely in either virtual effect: the repeat
can clear or grant it again. Keep lifecycle state transitions idempotent and
guarded by an explicit pending marker.
