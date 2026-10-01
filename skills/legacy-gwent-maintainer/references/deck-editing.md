# Deck counts and blacklist identity

Last verified: 2026-09-30

Evidence covers source and executable logic; AI Play Mode remains pending.

## A blacklist offers ordinary and premium copies as separate choices

- Symptom: the blacklist shows two versions of the same card, or a leftover
  premium-only filter presents premium cards as a distinct ban option.
- Cause: the blacklist reused collection version expansion even though
  `BlacklistModel.Blacklist` stores only card IDs. `Player.InBlacklist` compares
  those IDs with `opponent.Deck.Deck`; it never distinguishes appearance.
- Fix: `EditorInfo.CollectionVariants` uses `DeckCardCounts.BlacklistCandidates`
  for blacklist editing: distinct card IDs, explicit `IsPremium = false`, no
  dependency on version/ownership filters. Hide and disable the deck version
  controls while editing the blacklist, without changing saved preferences.
  Selected rows also render ordinary and insertion/removal treats identity only.
- Prevention: keep the existing two-distinct-gold-card limit. Do not add
  premium IDs, per-version ban fields, or require owning a premium to ban it.
  Returning to collection/deck editing must restore its normal version controls.
- Verification: executable checks invoke the actual compiled client selection
  method across all version/ownership filter combinations and both package
  capabilities. Actual `Player.InBlacklist` and `GwentRoom.InBlacklist` checks
  cover ordinary/premium copies, both room directions, and unrelated IDs.
  Control visibility, layout and border rendering still require Unity Play Mode.

## Copper quantities and top-level deck totals disagree

- Symptom: only premium copper displays a count, or a full mixed-version card
  still advertises unused inventory as addable copies.
- Cause: counter visibility was premium-only; inventory subtraction omitted
  the shared name cap. `DeckModel.PremiumCards` marks a subset of `Deck`, not
  additional copies or a separate quality.
- Fix: `CardCopyBadge.ShowsCount` permits either copper version, including
  standard-only packages. `DeckCardCounts` counts actual `Deck` entries and
  calculates addable copies from the minimum of remaining version inventory,
  shared name slots, total deck slots and gold/silver slots. Ordinary 2 plus
  premium 1 produces rows x2/x1, copper 3, and zero addable for either version.
- Prevention: do not count distinct UI rows or add `PremiumCards` to totals.
  Leaders are separate. Keep standard/special gold limits and blacklist limits
  intact; standard-only presentation must fold existing premium markers into
  one ordinary row without erasing the stored appearance choices.
- Verification: shared helper checks cover mixtures, removal, inventory, full
  decks, group caps and stale markers. Compile both package capability paths;
  an Editor-only compile does not cover the standard package.
