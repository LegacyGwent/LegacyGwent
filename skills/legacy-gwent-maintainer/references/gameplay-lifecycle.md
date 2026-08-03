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
   points, resolve spying, then send global `AfterUnitDown` and record history.
6. Resolve `BeforePlayStayCard` and any cards waiting in `PlayersStay`.
7. If the original unit remains on a row, raise `CardDownEffect`.

A unit removed during `CardPlayEffect` never reaches `CardDown` or
`CardDownEffect`. A concealed unit exits immediately after `ShowCardDown` and
does not follow the ordinary event sequence. Summon, resurrect, move, and manual
placement helpers may call different subsets; inspect their caller instead of
assuming they are equivalent to a hand play.

## Landing reactions

`CardDown` broadcasts `AfterUnitDown` only after the arriving card is shown,
points are updated, and spying is resolved. It then records history; the caller
later raises `CardDownEffect` if the original unit remains on a row. There is no
card-local post-deploy/pre-landing hook. State that must exist in hand, such as
the current Quen Shield, must be represented before `Play` begins rather than
attached at `CardDown`.

## Damage and shield

`Damage` sends `BeforeCardDamage`, adopts any redirected target/value/type, and
then checks `Card.Status.IsShield`. A shield clears itself and cancels the whole
damage instance before armor or lethal handling. This applies to every caller
of `Damage`, including duel, weather, trap, and hand damage; there is no native
duel-only shield exception.

`IsShield` is serialized client-visible state and is valid in hand or on the
board. `ToCemetery`, `Repair`, locking, and absorbing damage clear it. Quen uses
this ordinary state immediately while the chosen copies are still in hand/deck;
there is no Quen-origin marker or delayed landing hook.

## Power reduction is not one operation

- `Damage` subtracts temporary current power through `HealthStatus`. Shield
  consumes itself and cancels one complete `Damage` call before armor, including
  penetrating damage, weather, traps, hand damage, and each Duel hit. Repeated
  effects issue separate calls, so only one repetition is blocked.
- `Weaken` subtracts base `Strength`, capped at the target's remaining base
  `Strength`. It neither checks nor consumes Shield and survives cemetery
  `Repair`; reaching non-positive total power can kill or banish the unit. A
  base-1 unit with +5 Boost therefore receives only 1 of Cyprian Wiley's stated
  4 Weaken, remains on the board at 5 power with base `Strength == 0`, and is
  banished instead of firing `AfterCardDeath` the next time it enters the
  cemetery, including through Consume. Consume snapshots its 5 power before
  removal, so the consuming unit still receives that Boost. This is the engine
  operation meant by the strict card-text term “Weaken/削弱”.
- `Lower_Power_By` directly subtracts `HealthStatus` without `Damage`, armor, or
  damage events. It bypasses Shield and leaves it intact; current Amnesty uses
  this path to set a returned unit to 1 power.
- `Reset` assigns `HealthStatus = 0`. It can look like power reduction when it
  removes Boost, but it is neither Damage nor Weaken and does not consume
  Shield. Peter Saar Gwynleve and Mardroeme reset before applying 3 Weaken,
  whereas Cyprian Wiley applies 4 Weaken without resetting; their outcomes
  therefore differ on already Boosted or damaged units. Against the same
  base-1/+5 unit, Peter first removes the +5, then Weakens the remaining 1 base
  power to 0 and banishes it immediately. Direct status
  assignment, transformation, destroy, consume, and banish likewise do not
  become blockable merely because they reduce or remove displayed power.
- `Drain` calls penetrating `Damage` and then Boosts the source by the requested
  amount without checking how much damage landed. Shield prevents the target's
  loss but, in the current implementation, does not prevent the draining unit's
  full Boost.

## Duel and repeated deployment

The shared `Duel` helper deals the source's current power to the target, then—if
the target survives—deals the target's current power back, repeating until one
side leaves play or the safety limit is reached. Duel has no special Shield
handling: the normal `Damage` pipeline consumes the defender's Shield and
cancels that hit. If both units begin Shielded, the first attack and first
counterattack consume one Shield each, after which the next iteration makes
progress. Ice Troll passes a source-damage multiplier into this same helper for
its frost branch; do not reintroduce a parallel loop.

`Duel(target, source)` uses the effect owner's `Card` as initiator; `source` is
only attribution. Therefore forced Duel effects establish initiative by which
participant's effect invokes the helper. Treason deliberately invokes it from
the first selected participant.

Ice Troll deliberately calls ordinary self-`Damage(1)` before target selection
and Duel. This local cost consumes Quen/Shield without changing global Duel
semantics, triggers normal damage listeners, and can stop the effect if the
Troll leaves play. Its Biting Frost multiplier applies only after that cost, so
the Duel uses the Troll's resulting current power.

Syanna records both `AfterUnitPlay` and `AfterUnitDown`, then directly repeats
the target's `CardPlayEffect` and `CardDownEffect` at `AfterRoundPlay`. Do not put
one-shot state removal/restoration solely in either virtual effect: the repeat
can clear or grant it again. Keep lifecycle state transitions idempotent and
guarded by an explicit pending marker.

## Transformation and counters

- `Transform` replaces the card status and effect instance. Always assign the
  new effect to both `Card.Effects` and the primary `Card.Effect`; updating only
  the list leaves callers executing a stale pre-transform effect.
- Transformation does not preserve arbitrary old-form status automatically.
  Apply new-form Armor/Boost after the transform and explicitly implement any
  immediate form effect, as Old Speartip does.
- `TurnCardPlayedNum` is incremented before a played card's deploy effect. When
  text refers to cards played earlier in the turn, subtract or structure loops
  with the current card already counted; lock this with a zero-earlier-card test.
