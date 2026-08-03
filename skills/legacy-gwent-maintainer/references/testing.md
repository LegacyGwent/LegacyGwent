# Headless gameplay testing

Last verified: 2026-08-04

Load this reference before testing a card whose correctness depends on deploy,
selection, movement, death, landing, weather, duel, or chained events.

## Test layers

- `test/Cynthia.Card.Server.Tests` covers static rules, map/localization
  consistency, persistence boundaries, and service compatibility. Keep fast
  pure assertions here.
- `test/Cynthia.Card.Gameplay.Tests` is the isolated in-process gameplay suite.
  It references the production Common, AI, and Server projects, but does not
  start Unity, SignalR, MongoDB, a web host, or any other external engine.
- `test/AITest` is the original console proof that two AI players can drive a
  complete `GwentServerGame`. Treat it as the design ancestor, not the automated
  assertion suite.
- Workspace `headless-ai-probe` is a separate live integration probe. It uses a
  real server, SignalR, and MongoDB and is therefore not a unit-test substitute.

## Reusable fixture

`Cynthia.Card.Gameplay.Tests/HeadlessGameFixture.cs` creates the real
`GwentServerGame` with deterministic no-UI AI players. The players consume the
same server operations as a client and always choose the first eligible option,
so tests exercise production selection and event pipelines without random UI
input.

Build a scenario by adding physical `GameCard` objects to explicit zones, call
`SynchronizeClientsAsync`, then invoke the production play/effect method. For a
synthetic edge case, replace only the tested card's main `CardEffect` with a
small test-only effect; never add test branches or test card IDs to production
code.

## Commands and CI

Run the isolated suite with:

```powershell
dotnet test src/Cynthia.Card/test/Cynthia.Card.Gameplay.Tests/Cynthia.Card.Gameplay.Tests.csproj
```

The DIY-AI CI solution build includes this project and the server job runs it as
a separate `Run headless gameplay tests` step. Keep it deterministic and under a
few seconds; full random AI-vs-AI balance simulations belong in an opt-in tool,
not the required CI lane.

## Current lifecycle coverage

The suite verifies that Quen immediately Boosts and Shields same-ID cards in
hand/deck, blocks damage to a revealed hand unit, lets both Duel participants'
Shields block their first incoming hit, and resolves two Shielded units without
looping. Ice Troll coverage verifies its pre-Duel self-damage both with and
without Shield, plus the Biting Frost multiplier after that cost.

The August 3 scenarios cover non-stacking Living Armor including self, Ivo's
self-only Deathwish, Lady of the Lake's non-doubled count, Thaw's repeat count,
both Lonely Champion bonuses, repeated Princess turns, both Old Speartip
transforms, Aguara's non-spy hand filter, and the Magic Lamp/The Last Wish
cross-card dependency. Saesenthessis: Blaze coverage verifies cemetery refill
for non-Leader/non-Spying units only, exclusion of leaders, spies and specials,
resurrection events, continued draws, and empty-pool termination. Add future
lifecycle regressions here instead of approximating them with source text
assertions.
