# Headless gameplay testing

Last verified: 2026-08-07

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
  Before running it, point its Common and AI project references at the worktree
  under test and rebuild it. A stale probe model can complete and persist a real
  match, then report a false failure while deserializing newly added fields such
  as `GameResult.ModeId`.

## Reusable fixture

`Cynthia.Card.Gameplay.Tests/HeadlessGameFixture.cs` creates the real
`GwentServerGame` with deterministic no-UI AI players. Menu, place-card, and row
selection choose the first eligible option, so tests exercise production
selection and event pipelines without UI input. The inherited `PlayCard`
implementation may still choose a legal landing row randomly; avoid asserting a
specific row unless the fixture explicitly overrides that method. When a chain
needs a specific menu card, control the candidate order or add a queued card-ID
selector rather than relying on shuffled order.

Build a scenario by adding physical `GameCard` objects to explicit zones, call
`SynchronizeClientsAsync`, then invoke the production play/effect method. For a
synthetic edge case, replace only the tested card's main `CardEffect` with a
small test-only effect; never add test branches or test card IDs to production
code.

`GameCard` keeps its primary `Card.Effect` and its event-dispatching
`Card.Effects` entries as distinct effect instances. This is invisible to
stateless one-shot effects but matters for a private field that is initialized
on Deploy and consumed by a later event, such as Cloud Giant's remaining
Immunity turns. Exercise those cards through the real `Play` pipeline or raise
`CardPlayEffect` on `Card.Effects`; do not initialize state by calling
`card.Effect.CardPlayEffect(...)` and then expect `card.Effects.RaiseEvent(...)`
to observe the same private field.

Tests that inspect state immediately after a nested queued operation must enter
through `Game.AddTask`. Calling `Effects.RaiseEvent` directly while the game
operation list is idle drains nested tasks immediately and can hide production
timing where `Damage -> ToCemetery` is still queued. Reproduce every relevant
precondition too: a weather interaction test must actually set the target row's
weather before the card effect runs.

## Commands and CI

Run the isolated suite with:

```powershell
dotnet test src/Cynthia.Card/test/Cynthia.Card.Gameplay.Tests/Cynthia.Card.Gameplay.Tests.csproj
```

Run the Server and Gameplay projects sequentially in one worktree. They share
Common/AI build outputs, so concurrent `dotnet test` processes can race on
`obj/Release` and fail with CS2012 even when the code is correct.

An actively running local server also locks its copied Common/AI assemblies in
the Server output directory on Windows. Stop that exact local server process
before rebuilding or running the suites, then restart it from the same feature
manifest after tests. Repeated MSB3026/MSB3027 copy failures are an output lock,
not a gameplay regression.

## Card-batch preflight

Before the first push of a card batch, consolidate all known changes and pass
these gates locally:

1. For every changed or restored card ID, assert strength, group, derived/deck
   availability, effect type, and exact Chinese `Info` in the static suite.
2. Require the active `GwentMap` description to equal the server Chinese locale,
   then require server, Unity Resources, and Unity StreamingFile locale copies
   to agree for each supported language. Do not wait for live `GetCardMap` to
   discover source/locale drift.
3. Require the runtime user-card set and Mongo reset allowlist to be identical,
   including derived dependencies that must remain non-deckable.
4. Cover each lifecycle rule with a positive and a negative headless scenario.
   For example, a Gold unit entering through `Play` summons Roach, while the
   same unit entering through `Summon` does not.
5. Treat an interpretation not established by source or explicit user wording
   as an assumption. Surface it before release; never encode an unconfirmed
   assumption as a regression test and then treat that test as business truth.

Run both suites, knowledge validation, and `git diff --check` before the first
push. Production commit, `/healthz`, CardMap version, and SignalR map reads are
post-deploy confirmation, not substitutes for this preflight.

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

The August 4 scenarios verify Living Armor preserving the original An Craite
Greatsword through lethal-looking damage and allowing its engine to trigger;
Calanthe consuming positive Boost without going through the damage pipeline,
while leaving Armor and Shield untouched; Meve and Anna's restored DIY
behavior; Dana directly playing a Neutral Gold; and Dana chaining Royal Decree
while Roach correctly summons for a played Gold unit but not a summoned one.
The second-batch scenarios cover Gascon self-exclusion, Albastra wings and
two-turn Frost, Syanna's wounded-amount cadence, and Coën's deploy, tied-lowest
Witcher Boost, and opposite-row Farmer Deathwish. Static tests separately lock
the retired variant slots, runtime/Mongo allowlist equality, CardMap
`1.0.0.171`, source/locale agreement, leader-only draft validity, and Dana's
Addressables entries.

The August 5 scenarios cover Aard's Siege-row bonus, Professional's Deathwish-
free Banish, Olgierd's round-start return, Imlerith under Frost, unrestricted
Dettlaff cemetery rarities, per-copy Sir Scratch-a-Lot triggers, Cloud Giant's
Fog-duration Immunity, both Keltullis branches, and Tatterwing's all-unit row
movement.

The August 6 scenarios cover Ignis Fatuus's non-recursive Doomed copy and
idempotent Fog-to-Weaken conversion with Shield preservation; Cloud Giant's
owner-only turn-end damage and survivor movement; Gael's deploy plus exactly
one next-turn repeat; Red Rider's three Frost-death threshold and round-cleanup
exclusion; Water Hag's Boost/Strength split; Ogre Warrior's one-shot third-turn
check; Hybrid's Deathwish-before-Consume ordering; and Apiarian Phantom's
kill-gated Frost. When testing a card with private deploy-to-event state, raise
both `CardPlayEffect` and the later event through `Card.Effects`; calling the
separate legacy `Card.Effect` instance gives a false negative.

The August 6 Nilfgaard scenarios additionally cover two penetrating Assassination
hits, same-row non-adjacent Treason, both Hefty Helge reveal branches, deterministic
lowest-rarity revealing, restored Alba Pikeman turn cadence and Armor, Cupbearer
conceal/Boost cadence, Mage Infiltrator's board/hand/no-target branches,
representative odd-half rounding, and Xarthisius's move-and-lock. Apiarian
Phantom has a real-task-pipeline four-quadrant regression: Ignis present/absent
crossed with lethal/nonlethal exact-6 damage against a target already under Fog.
Lethal cases replace Fog with Frost; nonlethal cases retain Fog. Accept either
off-board state or non-positive power after damage because the cemetery move may
be pending or already repaired.
