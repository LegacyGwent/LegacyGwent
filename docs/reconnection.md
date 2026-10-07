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

Run from the repository root with .NET 8 or later:

```
dotnet build src/Cynthia.Card/src/Cynthia.Card.Server/Cynthia.Card.Server.csproj
dotnet run --project src/Cynthia.Card/test/ReconnectRegression/ReconnectRegression.csproj
dotnet run --project src/Cynthia.Card/test/ClientLifetimeRegression/ClientLifetimeRegression.csproj
```

The server regression runs against production code and covers interrupted
claims, frozen wire payloads, answers during blocked snapshot transport, stale
owners, obsolete tickets and competing terminal paths. The client regression
links the production LocalPlayer with SignalR 3.1.5, checks retirement and fresh
receive isolation, and parses the client service/scene loop using C# 7.3.

These are offline checks. They do not verify Unity native execution, real
two-client reconnects, mixed client versions or deployed behavior. New platform
packages still need those acceptance checks before being advertised as tested.
