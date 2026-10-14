# Reconnection ownership and validation

The server holds a disconnected player's running game for 60 seconds. The
client retries for 55 seconds and uses a fresh local player instance for the
restored scene. This feature requires a server and client built with the
reconnection changes; old clients do not implement automatic resume.

## Shared invariants

- A live session and a reconnect ticket are claimed atomically, inside the same
  game execution segment that captures the restored state. A disconnected or
  replaced owner cannot claim the game or submit an answer.
- Game continuations, input acceptance, restored state and settlement use the
  game's serialized context. State and pending requests are eagerly serialized
  without an intervening await, then sent as an ordered batch. Accepted answers
  consume their pending request at this boundary.
- Natural and forced settlement share one completion signal. Cleanup waits for
  settlement, and input waiting stops after the game ends. An obsolete timeout
  cannot expire a replacement ticket.
- Retired Unity players cancel queued transport, unsubscribe hub handlers,
  release their receive loop and reject stale scene callbacks. Async operation
  continuations check retirement before accessing the old scene; an old scene's
  failure cannot reset the newly restored session.

## Checks

Run from the repository root with .NET 10:

```
dotnet build src/Cynthia.Card/src/Cynthia.Card.Server/Cynthia.Card.Server.csproj
dotnet run --project src/Cynthia.Card/test/ReconnectRegression/ReconnectRegression.csproj
dotnet run --project src/Cynthia.Card/test/ClientLifetimeRegression/ClientLifetimeRegression.csproj
dotnet run --project src/Cynthia.Card/test/MessageBoxLifetimeRegression/MessageBoxLifetimeRegression.csproj
```

The server regression runs against production code and covers interrupted
claims, frozen wire payloads, answers during blocked snapshot transport, stale
owners, obsolete tickets and competing terminal paths. Real mulligan entry
checks block automatic transport flush during both opening and request sends:
the snapshot must contain the current hand, remaining count and request before
any send can suspend, and an early accepted answer must stay consumed. The client regression
links the production LocalPlayer with SignalR 5.0.8, checks retirement and fresh
receive isolation, and parses the client service/scene loop using C# 7.3.

The message-box check links production `MessageBox` with minimal Unity stubs.
It covers overlapping confirmations, notification replacement, stale countdowns,
close/disable and stray clicks. Every display owns an independent answer;
replacement declines the old confirmation and can never synthesize consent.
Native layout/timer rendering is not exercised by the stubs.

These are offline checks. They do not verify Unity native execution, real
two-client reconnects, mixed client versions or deployed behavior. New platform
packages still need those acceptance checks before being advertised as tested.

## DIY-AI adaptation

The AI track keeps its existing settlement gate, stable match/round reward keys,
GG receipts and asynchronous daily reward queue. Reconnect sees that same
terminal flag; it does not create another result or reward path. Retired client
transport remains separate from the fresh session.

Game callback exceptions are logged without poisoning later serialized actions.
Awaited gameplay failures remain observable by the match owner. Initialization,
gameplay and cleanup are covered by the match task boundary. A failed incomplete
match is cleaned up without inventing a normal draw, result, rank or reward.
Already committed round rewards retain their existing durable retry semantics.

The focused reconnect fixture also checks callback isolation and failed initial
notifications. `SurrenderCrownTest` covers the AI terminal/reward invariants.
Its wallet is a fixture; it does not validate MongoDB or deployed reward jobs.

## Local loopback acceptance

```
dotnet run --project src/Cynthia.Card/test/ReconnectLoopbackAcceptance/ReconnectLoopbackAcceptance.csproj
```

This test starts Kestrel only on an ephemeral loopback port and maps the actual
`GwentHub` with the production JSON converter/protocol. Production `ClientPlayer`
creates a selection request; SignalR 5.0.8 and the linked production `LocalPlayer`
consume it. Closing that real connection triggers production disconnect/ticket
creation. A fresh connection invokes production `Reconnect`, consumes typed
snapshot/pending arguments, and responds to finish the original server wait.
On a candidate containing the spectator port it also consumes `SetEnemyDeck=44`
through the production spectator channel; otherwise that check reports SKIP.

The fixture pre-registers isolated users/room in memory, bypassing credential
Login, MongoDB and reward persistence. It does not start full production Startup
or its hosted workers. `StopAsync` is graceful WebSocket closure, not OS packet
loss; Unity scenes, reconnect retry UI, mixed packaged clients and deployed
network-outage behavior remain separate acceptance boundaries. The spectator
check validates wire consumption, not spectator admission or privacy policy.

## Reward fixtures and serialized callbacks

`RoundWon` is a synchronous enqueue callback. A fixture that calls an async
Mongo method and blocks on `GetResult()` inside it deadlocks under the game
synchronization context: the callback blocks the drain while the awaited
continuation waits for that same drain. This is a fixture contract violation,
not a reason to bypass production game serialization.

`RewardSystemTest/CrownRewardCapture.cs` records stable event identities in the
callback. Fixtures await the actual database jobs outside the game action before
asserting persisted crowns. Game completion and persistence waits are bounded;
background tasks are retained and awaited so storage errors cannot strand a
completion signal. The shared helper is also linked into `ReconnectRegression`
with a genuinely asynchronous, database-free continuation and failure check.
That check establishes scheduling behavior, not Mongo persistence acceptance.
