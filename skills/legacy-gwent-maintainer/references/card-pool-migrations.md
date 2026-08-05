# Card-pool migrations

Last verified: 2026-08-05

Verified against the DIY-AI reset lineage through `1.0.0.172`.

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
- Count map entries from dictionary `CardId` keys (or the initialized
  `CardMap`), never by counting `new GwentCard()` expressions in
  `GwentMap.cs`. The master file contains one additional constructor in
  `DeckChange` for cloning card metadata; constructor counting reports 517 even
  though the baseline map contains 516 IDs.
- Add `CardMapVersion`/`RulesetId` to new result and deck records. Version-gate
  analytics such as `QueryCard`; old unversioned codes must use the frozen
  `1.0.0.153` map or fall back to result summaries.

## DIY-AI baseline classification

- Current map: 716 entries. `origin/master` contributes the 516 baseline IDs.
- Keep system card `70014` (Goddess of Justice) and AI-only IDs `70018`,
  `80001`, `80002`, `80003`, `89004`, `89005`, `89006`, `89007`, `89008`.
- After the August 5 batch, 142 entries are retired. Eight more DIY cards are
  restored to user decks: `70084`, `70102`, `70113`, `70145`, `70154`, `70164`,
  `70170`, and `70177`. Original card `22003` is not in the retirement manifest
  but is derived-only, so it must still be excluded from the exact migration
  allowlist while `22001` remains deckable.
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

  New leader `70191` is appended after those six historical variant slots.
- The active exceptions also include restored leaders Meve `70045`, Anna
  Henrietta `70149`, Queen Calanthe `70179`, and new Dana Meadbh `70191`.
- The active exceptions further include the eight August 5 restored cards above;
  `DiyAiCardPool.IsUserDeckCard`, rather than only the retirement manifest, is
  authoritative because derived original cards such as awakened Old Speartip
  are unavailable without being retired.

## Retirement checklist

1. Keep all 716 CardMap keys in the same order and maintain an explicit retired
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
6. On Mongo `28021`, stop only `card-diy-ai` and take a restorable dump before
   mutation. For a retired test card with a compatible successor, prefer an
   explicit old-ID-to-new-ID migration inside decks and blacklists; if the
   successor is already present, remove only the obsolete duplicate rather than
   invalidating the whole deck. Use whole-deck removal and starter seeding only
   when no safe card-level migration exists. Preview counts, archive affected
   documents, execute, and verify. Never touch DIY port `28020` or service
   `5005`.
7. Preserve historical results. Harden any direct `GwentMap.CardMap[id]` lookup
   with version-aware decoding or `TryGetValue` before the retirement ships.

## Old Speartip migration ID trap

- Symptom: replacing derived-only `22003` with `22004` makes affected decks gain
  Caranthir instead of the deckable sleeping form of Old Speartip.
- Cause: adjacent Monster gold IDs were inferred instead of verified against
  `GwentMap`: `22001` is Old Speartip: Asleep, `22003` is Old Speartip: Awakened,
  and `22004` is Caranthir.
- Fix: migrate deck and blacklist references from `22003` to `22001`; when
  `22001` already exists, remove only the obsolete `22003` copy.
- Prevention: every card-level database replacement must name both source and
  target cards in review notes and verify both IDs against the active map.
- Verification: use a pre-migration dump to enumerate every source deck, then
  require a post-check with zero pending replacements, missing decks, and
  unresolved decks before restarting `card-diy-ai`.

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
