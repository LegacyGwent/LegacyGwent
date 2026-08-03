# Card-pool migrations

Last verified: 2026-08-03

Verified against the DIY-AI reset lineage through `1.0.0.166`.

Load this reference before removing, hiding, renumbering, or restoring cards.

## CardMap order is persistent data

- `CompressDeck` stores each card as its ordinal in `GwentMap.CardIdMap`, not as
  the card ID. `DeCompressToDeck` resolves that ordinal through
  `CardIdIndexMap`.
- Existing `gameresults` and `aigameresults` store compressed deck codes without
  `CardMapVersion` or `RulesetId`. Removing or reordering a CardMap entry silently
  remaps every later ordinal and corrupts historical decoding.
- Never remove, reorder, or reuse an existing CardMap slot. Retire a card
  logically, preserve the complete map for decoding, and append future cards.
- A published new ID remains permanent map history even when an experiment ends.
  Retire it logically; deleting it would corrupt decks and results created during
  the experiment. Sharing `CardArtsId` does not change this identity rule.
- Before changing availability, snapshot the exact ordered ID map and increment
  `CardMapVersion`. Existing Unity clients fetch the server map when the version
  differs, so a pure availability reset does not by itself require a rebuilt
  client.
- Add `CardMapVersion`/`RulesetId` to new result and deck records. Version-gate
  analytics such as `QueryCard`; old unversioned codes must use the frozen
  `1.0.0.153` map or fall back to result summaries.

## DIY-AI baseline classification

- Current map: 715 entries. `origin/master` contributes the 516 baseline IDs.
- Keep system card `70014` (Goddess of Justice) and AI-only IDs `70018`,
  `80001`, `80002`, `80003`, `89004`, `89005`, `89006`, `89007`, `89008`.
- After the August 3 batch, 159 entries remain legacy DIY retirement candidates.
  The promoted user-deck exceptions include `70001` (昆恩法印), `70041`
  (鬼针草煎药), `70042` (合欢茎魔药), plus the 18 deckable IDs listed in
  `card-rules.md`. The generated dependencies `70006`, `70071`, and `70162`
  must be unretired but remain derived/non-deckable. The two potions use exactly
  the `Special + Alchemy` categories.
- Do not classify only by the `GwentMap.cs` DIY marker. The marker partition and
  `origin/master` swap `70084` and `13015`: the master-ID rule retires `70084`
  and retains `13015`.
- The DIY-AI reset ruleset restores ordinary original-card definitions and
  effects from `origin/master`. Preserve the current DIY-AI implementation for
  the actual AI0-AI5 dependency closure, Goddess of Justice, and first-player
  decision/compensation mechanism.
- Compare against the frozen pre-reset DIY-AI commit
  `fb174665c109e73da49c8a239cc78077faf6cfc1`. A moving `origin/diy-ai`
  reference becomes the reset state after push and makes a rerun select no
  restoration targets.
- Gold, silver, and copper weather are an explicit rules exception: retain the
  DIY-AI definitions, effects, and every `RowEffect` implementation. In
  particular, Torrential Rain damages one random lowest unit and one random
  highest unit and keeps its Otkell bonus; Ale of the Ancestors repeats Golden
  Froth when moved and damages itself by 4.
- Weather row colors are serialized in Unity
  `Assets/Resources/Scenes/GamePlay.unity` on the `ShowWeather` component as the
  aligned `WeatherIndex`/`WeatherColor` arrays. Do not infer or reconstruct the
  gold-weather color in server code; preserve the DIY-AI scene values and verify
  both arrays together.

## Retirement checklist

1. Keep all 715 CardMap keys in the same order and maintain an explicit retired
   ID manifest. Make retired cards non-deckable/hidden while keeping metadata for
   history.
2. Reject unknown, derived, and retired IDs on deck upload, deck-code import, and
   match start. Do not rely on the client; `AddDeck` historically skipped its
   `IsBasicDeck` validation. Database cleanup must use the exact active-user-card
   allowlist, not only the retired/system denylist: orphan IDs such as `89009`
   and `89010` are absent from both CardMap and the retirement manifest.
3. Remove retired IDs from retained cards' linked/discovery pools and direct
   effect references. Also replace retired IDs in starter decks before allowing
   account creation.
   Treat downloaded locale data as a separate rules surface: the Chinese client
   must receive names and descriptions overlaid from the active `GwentMap`, not
   stale DIY text left in `Locales/cn.json`. Because legacy clients cache locales
   against `CardMapVersion`, every change to a card `Name`/`Info` or localized
   card description must increment that version in the same commit, even when
   card IDs, order, and effects are unchanged. A missing bump blocks release.
4. Preserve `70014` and `DecideRedCoin`; it implements the first/second-player
   bid and compensation. Preserve the AI dependency closure and smoke-test all
   advertised AI queues.
5. Do not delete art mechanically. Some promoted DIY definitions share a
   `CardArtsId` with retained originals; keep retired art as reusable inventory.
6. On Mongo `28021`, stop only `card-diy-ai`, take a restorable dump, archive the
   affected deck documents, remove invalid decks and blacklist entries, seed a
   pure-baseline starter deck for accounts left with none, then deploy and
   verify. Never touch DIY port `28020` or service `5005`.
7. Preserve historical results. Harden any direct `GwentMap.CardMap[id]` lookup
   with version-aware decoding or `TryGetValue` before the retirement ships.

The 2026-08-01 production migration on Mongo `28021` scanned 41,940 users,
removed 18,322 invalid decks across 3,136 users, seeded 562 users with the
original starter deck, and removed 25 invalid blacklist entries across 24
users. Its immediate post-check reported zero remaining invalid decks and
blacklist entries. The restorable pre-migration dump is under
`/var/backups/legacy-gwent/diy-ai-card-reset/20260801T155718Z.lSCs8t` and has
`state=complete`. A stricter allowlist audit then found three historical decks
containing orphan IDs `89009`/`89010`. The corrective migration removed those
three decks, seeded all three affected users, and completed with zero remaining
unknown, derived, retired, or system-card deck/blacklist references. Its backup
is `/var/backups/legacy-gwent/diy-ai-card-reset/20260801T162135Z.5oQj5e` with
`state=complete`. Future migrations must recompute immediately before execution
because the public server remains live.
