# Account message queues

Last verified: 2026-10-06

## Season confirmation loops

- Symptom: an old account with pending season notices repeatedly sees the same
  confirmation after clicking Yes. Empty rewards and rank zero use the same
  queue as ranked notices; neither is an acknowledgement condition.
- Cause: Unity sent `User.UserName` to a read path filtering `PlayerName`, while
  removal filtered `UserName`. Different identities and cross-account name
  collisions can read another account's notice. Offline settlement also passed
  `PlayerName` into a save path filtering `UserName`. Separately, removal was
  not awaited before rereading, display helpers were not awaited, and complete
  array replacement could overwrite concurrent sends or acknowledgements.
  Successful removal also returned false.
- Fix: retain legacy hub signatures, but read the connection owner's login.
  For removal, require the wire login to equal that owner as a precondition,
  then route through the owner; never use caller names to select another account.
  Offline settlement resolves `PlayerName` and writes the resulting immutable
  document ID. Append atomically with `$concatArrays/$ifNull`; acknowledge with
  atomic `$pull` on envelope ID. Unknown payload types do not need deserialization
  merely to acknowledge an ID.
- Prevention: `UserName` is the login and `PlayerName` the visible nickname.
  Reads, display ownership, and acknowledgements must refer to one account.
  The singleton reader checks on each main-menu entry, serializes checks and
  display, awaits acknowledgement before rereading, and stops if the same
  envelope remains instead of reopening it. Capture the login across waits and
  send that captured owner to the hub; changing accounts cannot acknowledge a
  previous owner's popup.
- Verification: `UserMessagePersistenceTests` uses disposable MongoDB 4.4.29 on
  loopback 28163. Cases cover different/colliding names, several old notices
  without rank/rewards, repeat acknowledgements, 16 concurrent senders alongside
  acknowledgement, null/missing queues, and hub ownership. `ClientMessageQueueTests`
  compiles the production Unity reader against minimal UI/transport fakes and
  covers delayed acknowledgements, reentry, failed acknowledgement, Standby
  events, account changes, and immediate live dispatch.

## IDs across empty queues and upgrades

- `gwentdiy.usermessagecounters` stores `_id = UserInfo.Id` and `Sequence`.
  Its built-in unique `_id` index is sufficient; no user-document field or extra
  index is added. Keep the collection when backing up account data.
- Seed using `$max` over existing envelope IDs before either the first upgraded
  append or acknowledgement. Seeding only on append loses the legacy high water
  mark if acknowledgement drains the queue first. Atomic `$inc` allocates the
  next ID. Concurrent initial upserts retry duplicate `_id` as a non-upsert
  `$max`. Failed appends may leave harmless ID gaps.
- Acknowledgement is idempotent for an existing account and a removed ID. Never
  reuse IDs after emptying the queue. Tests drain old IDs before the first new
  send, then replay old acknowledgements.
- User documents and the pipe-delimited legacy message format stay unchanged.
  Old servers can read accounts after rollback, but old writers ignore the new
  high water mark and can reuse IDs. Monotonic IDs are not guaranteed under mixed
  or rolled-back old writers. New-client handling tolerates old servers returning
  false after removal; it cannot fix old-server identity selection.

## Unity SignalR invocation dispatch

- Unity uses SignalR client 5.0.8, whose invocation dispatcher awaits handlers
  serially. A live notification handler must finish immediately and hand UI work
  to an exception-observed task; waiting for Standby or a button inside
  `HubConnection.On` blocks game callbacks needed to make progress.
- Keep UI serialization in the reader's display gate. Unsubscribe a Standby
  listener before completing its wait; completion can resume synchronously.
- `LiveSignalRHandlerCompletesBeforeStandbyOrUserInteraction` exercises the
  production registration delegate while game state and popup are both pending.

Run the opt-in persistence cases only against the disposable instance:

```powershell
$env:LEGACYGWENT_SEASON_TEST_MONGO = 'mongodb://127.0.0.1:28163'
dotnet test src/Cynthia.Card/test/Cynthia.Card.Server.Tests/Cynthia.Card.Server.Tests.csproj
```

The attribute skips these cases unless the exact disposable URI is set. They
create and remove only synthetic per-test documents, never real user records.
