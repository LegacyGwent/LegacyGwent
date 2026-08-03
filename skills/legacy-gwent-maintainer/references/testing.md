# Headless gameplay testing

Last verified: 2026-08-03

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

The initial suite verifies that Ice Troll receives delayed Quen only after a
survived deploy duel, that lethal duel damage leaves the pending marker
unconsumed, and that returning to hand during deployment also leaves it pending.
It also verifies Saesenthessis: Blaze exhausting its original deck, resurrecting
only cemetery units into the deck, firing resurrection behavior, continuing the
remaining draws, and stopping safely when no units can refill it. Add future
lifecycle regressions here instead of approximating them with source text
assertions.
