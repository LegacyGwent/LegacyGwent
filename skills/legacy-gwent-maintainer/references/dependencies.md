# Dependencies

Last verified: 2026-08-01

## Maintained boundaries

- Server and server-side tools target .NET 10; Common and AI remain
  `netstandard2.0`, and Unity remains 2019.4.1f1.
- Keep the Unity SignalR client at 5.0.8 during server and database-driver
  upgrades. CI checksum-protects the bundled assembly.
- The server pins `NLog.Web.AspNetCore` 4.9.3 and `MongoDB.Driver` 3.9.0.
  MongoDB.Driver 3.9 resolves MongoDB.Bson 3.9 and SharpCompress 0.48.1.
- `NLog.Web.AspNetCore` 4.9.3 resolves `NLog` core 4.7.2. Treat the web
  integration and core package versions separately; do not report NLog core as
  4.9.3.
- `DIY-AI CI` audits the complete solution, including transitive packages, and
  fails through `scripts/check-vulnerable-packages.py` on any known advisory.

## MongoDB driver migration rule

- For a 2.x-to-3.x upgrade, compile once on MongoDB.Driver 2.30.0 because that
  release marks APIs removed in 3.x. It is a migration checkpoint, not a
  deployable security destination.
- The final security baseline is MongoDB.Driver 3.9.0. It removes the old
  driver advisory and pulls SharpCompress 0.48.1, which fixes both known path
  traversal advisories.
- Never force SharpCompress 0.48.1 underneath MongoDB.Driver 2.30. The older
  zlib implementation calls a removed SharpCompress API; this can compile yet
  fail only when compression is used.
- LINQ3 is mandatory in Driver 3.x. The repository's highest-risk surface is
  its many `AsQueryable()` calls in `GwentDatabaseService`,
  `ScheduledEventService`, `DiyPage/Command`, and ConsoleTest.

## Server compatibility and lifecycle

- MongoDB.Driver 3.9 remains compatible with MongoDB Server 4.4, but MongoDB
  4.4 reached end of life on 2024-02-29. Compatibility does not mean the server
  is supported; plan a separate server upgrade before its driver compatibility
  window closes.
- Upgrade the driver without a BSON migration. Sample document types and
  collection counts before and after; investigate any GUID, decimal,
  DateTimeOffset, serializer, or LINQ translation change before deployment.

## Required verification

Run against the isolated DIY-AI MongoDB on 28021, never stable 28020:

1. Restore and Release-build `src/Cynthia.Card/Cynthia.Card.sln`.
2. Run `dotnet list ... package --vulnerable --include-transitive --no-restore`
   and require zero findings.
3. Start the candidate on an unused port and verify `/healthz` plus a real
   SignalR 5.0.8 connection.
4. Exercise registration uniqueness, login, deck insert/update/delete,
   season/ranking queries, normal and AI result writes/reads, DIY review
   insert/vote/comment updates, and a zlib-compressed Mongo read/write.
5. Remove every temporary QA document and confirm zero matches remain.
6. Before deploying dependency changes, finish a real Unity login, deck, and AI
   match regression against the preceding framework-only release.

The verified 2.13 -> 2.30 -> 3.9 migration built the full solution with zero
errors. The isolated runtime suite passed every operation above and left no QA
records; treat that as the minimum evidence for future driver upgrades.
