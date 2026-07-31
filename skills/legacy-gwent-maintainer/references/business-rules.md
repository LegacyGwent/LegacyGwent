# Business rules

Last verified: 2026-07-31

## DIY-AI release identity

- The player-facing product name is `DiyGwent AITest`. `AITest` intentionally
  lowers expectations: this is an experimental, aggressively changing track,
  not a promise that it is stronger or more stable than DIY.
- Use `DiyGwent-AITest-<platform>-<version>` for distributed client filenames.
  Keep internal branch, service, database, and deployment names as `diy-ai`.
- Preserve the distinct Unity product name as well as the Android package ID;
  the former isolates Windows PlayerPrefs/persistent data and the latter lets
  both Android clients coexist.

## Account identity

- Registration form naming is counterintuitive: stored `UserName` is the login;
  stored `PlayerName` is the visible in-game name.
- Verify account creation in MongoDB rather than trusting only the client screen.

## Deck validity

- Use `Cynthia.Card.GwentDeck.IsBasicDeck(...)` as the source of truth.
- A known valid basic deck has 25 cards with leader, rarity, and copy limits
  enforced by that method; do not infer validity from UI appearance alone.

## AI matchmaking

- `ai1` selects `SoldierTrainAI`, whose deck is based on repeated recruit card
  `89008` and whose bidding behavior is predictable.
- Forced-AI suffixes `#` and legacy `#f` are normalized by
  `GwentServerModels/GwentMatchs.cs` on `diy-ai`.
- UI instructions and server password parsing have diverged before; verify a
  match actually starts and inspect the server parser when they disagree.

## Match verification

- A real AI result is persisted in MongoDB collection `aigameresults` with
  player names, deck names, round scores, win counts, surrender state, and
  balance point.
- Prefer the persisted result plus the Unity victory screen for end-to-end proof.

## Card art inventory

- Run `scripts/card_art_inventory.ps1` from the skill directory; it compares
  full-size Addressable art, `GwentMap` dictionary keys and art IDs, explicit
  `CardEffectId` attributes, and `_slot` miniatures.
- Verified inventory: 1,343 unique full-size card-art assets, 709 `CardMap`
  entries, 692 explicit card-effect IDs, and 671 unique art IDs assigned to a
  card definition.
- Of the art assets, 659 are used by a card with an explicit effect, 12 are
  assigned only to effectless card definitions, and 672 are completely
  unassigned. Therefore 684 have no explicit effect, but only 672 are cleanly
  free for a new card without reusing an existing definition's art.
- Of the 672 unassigned art assets, 539 already have a matching `_slot`
  miniature and 133 need a miniature before the deck-list UI can use them
  cleanly.
- The `/cardart` authoring page enumerates a separate set of 1,049 small
  `wwwroot/scale` previews. It has 531 IDs not referenced by `CardMap`, but 499
  of those lack a Unity full-size image; only 32 overlap the clean full-art pool,
  and only one of those already has a registered miniature. Do not equate an
  available web preview with a client-ready card-art chain.
- Count occupation by the dictionary key, not the duplicated `GwentCard.CardId`
  property. Entry key `70108` currently has the incorrect property value
  `70106`; treating the property as identity miscounts effect/art ownership.
