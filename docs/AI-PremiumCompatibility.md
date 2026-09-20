# DIY-AI premium-card compatibility

This change adds premium-card presentation, meteorite powder, and daily rewards
to `diy-ai` while preserving the existing card IDs, deck rules, SignalR
operations, and AI card pool.

## Compatibility contract

- A premium card is the existing card ID plus the additive `IsPremium` field.
  It is never registered as another card ID. Legacy JSON readers ignore the
  field and continue to render the ordinary card.
- `DeckModel.PremiumCards` and `DeckModel.PremiumLeader` are nullable additive
  wire fields. Their absence means that the caller does not understand deck
  appearance; it does not mean "clear all premium choices".
- MongoDB `user.Decks` keeps the legacy BSON shape. Premium choices live in the
  separate `premium_collection.DeckSelections` map. Rolling the server back to
  a build without premium cards therefore does not require rewriting user or
  deck documents.
- When an old client modifies a deck, the server reconciles the stored premium
  choice against the new card list in the background. Choices for remaining
  copies are retained, removed copies are trimmed, and the leader remains
  premium only when the leader ID is unchanged.
- An old client does not fetch `GetPremiumCollection`. Login, registration,
  ordinary deck writes, matchmaking, and round progression do not wait for the
  premium wallet or reward workers. A wallet failure must not prevent ordinary
  play.

Both content variants share the same network and account contract. The build
toggle excludes animated payload, but does not hide premium UI or crafting.
Clean-checkout premium packaging still needs a reproducible content source and
CI preparation/matrix wiring; see [the packaging audit](AI-ClientPackagingAudit.md).

## Server-owned powder and daily rewards

`InitialPowder.json` controls the one-time account grant:

```json
{
  "Enabled": true,
  "Amount": 5000
}
```

The server records both `InitialPowderGranted` and the stable receipt ID
`initial-meteorite-powder-v1`. On startup a background service scans existing
accounts; registration requests another scan. Retries are idempotent. Disabling
the option pauses grants without consuming eligibility. No client grants this
powder.

`DailyQuests.json` grants 20 powder for the first server-observed activity of a
China-calendar day and grants 25, 35, and 45 powder at 2, 4, and 6 round wins.
The server checks the China midnight boundary for users who stay connected, so
they do not need to reconnect. Round keys remain in a cross-day processed-key
ledger to prevent delayed or repeated settlement from paying twice.

Round completion only queues reward work. The game advances immediately. The
worker persists `daily_round_reward_jobs` before changing the wallet, retries
failures, and uses the stable match/round key for idempotency. Client
notifications are best effort and do not control the database commit.

## Release checks

Before deploying `diy-ai`:

1. Build `src/Cynthia.Card/src/Cynthia.Card.Server/Cynthia.Card.Server.csproj`.
2. Run `Cynthia.Card.Server.Tests`, `DailyQuestTest`,
   `PremiumCraftingTest`, and `RewardClientTest` against an isolated MongoDB.
3. Confirm `CardLocales` and the operation/card-map protocol are unchanged from
   the target `diy-ai` commit.
4. Build both Unity packages and run one old-client login/deck/match probe plus
   one premium-client craft/select/match probe against a staging copy of the AI
   database.
5. Check `/healthz`, then verify that the initial-grant scan and pending reward
   jobs converge before treating the deployment as complete.
