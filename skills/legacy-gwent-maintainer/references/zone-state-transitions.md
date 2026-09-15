# Zone transitions and state cleanup

Last verified: 2026-09-15

Verified against the DIY-AI engine used by CardMap `1.0.0.198`. These are
current implementation rules, not a proposal to unify the helpers. Source:
`src/Cynthia.Card.Common/CardEffects/CardEffect.cs` and
`src/Cynthia.Card.Server/GwentServerModels/GwentServerGame.cs` below
`src/Cynthia.Card/`; proof is `ZoneTransitionStateTests` in Gameplay.Tests.

## Cleanup is distinct from unlocking

- `Repair()` clears Armor, HealthStatus (Boost and damage), IsCardBack,
  IsResilience, IsShield, IsSpying, Conceal, IsReveal, and IsImmue. It preserves
  IsLock unless explicitly passed `true`.
- Both Repair forms preserve base Strength, including prior Strengthen/Weaken,
  IsDoomed, Countdown/IsCountdown, and the existing effect objects/private
  accumulated fields. They do not restore the initial card definition.
- `Repair(true)` directly clears the lock field without AfterCardUnLock.
  `Resurrect` instead calls `Lock(source)` when locked, sending AfterCardUnLock;
  it never calls Repair. Unlocking alone does not clear other temporary state.
- Among status fields, `Reset` clears only HealthStatus; it also emits
  AfterCardReset and banishes a Unit with nonpositive base Strength.
  `Transform` creates a new CardStatus
  and new effect instance(s), after which the caller may add explicit settings.
  Neither term is interchangeable with ordinary movement or resurrection.

## Scenario map

| Scenario | Native behavior / actual card examples |
| --- | --- |
| Board to cemetery | Normal ToCemetery invokes Repair(), so temporary state clears but lock remains. Doomed and Tokens end banished. Raw ShowCardMove to cemetery does not perform this death/cleanup pipeline. |
| Cemetery Summon to board | No Repair or unlock; Summon clears reveal. Same-side landing preserves the remaining marked state. |
| Cemetery Resurrect to board | Unlock without Repair; successfully moving sends AfterCardResurrect. Effects such as Draug transform first, so their state change cannot represent generic resurrection. |
| Board replay | Play alone does not Repair or unlock. Decoy, Geralt Axii and Shupe's replay branch explicitly Repair(true), then enqueue/play and apply their own rewards. There is no universal Replay helper. |
| Board to deck | Princess Pavetta, Milva, Cintrian Field Medic, Queen Calanthe and Lyrian Landsknecht explicitly Repair(true). The Great Oak returns its played Dryad through raw movement without that cleanup. |
| Cemetery return to deck | Assire, Zoltan's Company and Swamp Thing use Repair(), preserving lock. Vandergrift's Blade no longer has a return-to-deck ability. |
| Cemetery Resurrect to deck | Unlock without Repair and emit resurrection events despite no battlefield entry. Dimun Smuggler, Nenneke and Saesenthessis: Blaze use this; Ciri Dash also adds its own Strengthen. |
| Hand to deck | Swap/mulligan clears reveal but preserves lock and power changes. Raw movement does not even clear reveal. |

Card-specific listeners can make additional changes after any native helper.
In particular, resurrection into Stay followed by Play also executes the normal
play pipeline. Do not infer behavior solely from the source/destination zones.

## Other transitions and important boundaries

- Hand/deck discard uses ToCemetery/Repair: temporary state clears, lock stays.
- Board to hand depends on the card: Emhyr uses Repair(true); Amnesty toggles
  Lock, sets current power to 1, then moves directly. Neither is a generic
  automatic property of returning to hand.
- Same-side board Move preserves state. Charm/cross-side placement also updates
  spying as part of landing; it is not wholesale state cleanup.
- Resilient round survival clears Armor and positive HealthStatus, consumes
  Resilience, but retains negative HealthStatus (damage). It does not Repair.
- The server's SendEvent skips locked cards in all zones. A locked Ulle,
  Sigvald, Morkvarg or Ciri Dash cannot use its own suppressed event to invoke
  resurrection and unlock itself; an external effect must actually call it.
- Native Resurrect unlocks before checking target-row capacity. A full-row
  failed attempt can therefore unlock without moving or sending resurrection.
  Ulle/Sigvald check for a free location before attempting automatic revival.
  This observed engine edge is documented, not changed by this card batch.

## Verification discipline

- Symptom: reports conflate remaining lock with all status being retained, or
  infer resurrection cleared a Boost already removed by earlier cemetery entry.
- Cause: Repair, unlocking, raw movement, Play and Resurrect are different paths;
  individual cards explicitly combine them and may add their own effects.
- Fix: identify the concrete helper and card, then distinguish each field's
  change. Do not modify a shared helper merely to match an assumed universal
  zone-reset rule.
- Prevention: test ordinary lifecycle transitions as well as deliberately marked
  synthetic states; report which is being tested. Preserve permanent power and
  counter assertions, and verify explicit card-level Repair calls separately.
- Verification: ZoneTransitionStateTests covers Repair's optional unlock,
  cemetery cleanup, Summon, Resurrect to board/deck/Stay, failed full-row revive,
  raw zone moves, direct Play versus Decoy, Pavetta, Nenneke, Swap, and retained
  Strengthen/Weaken. SeptemberThirteenthSmugglerTests additionally proves live
  resurrection listeners react to Smuggler's deck destination without Deploy.
