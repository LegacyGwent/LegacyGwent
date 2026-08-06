# DIY-AI Rule Card Framework Specification

Status: local design and implementation target. Do not publish or deploy until
the acceptance gates in this document pass.

## Purpose

Rule cards are a server-driven extension boundary for gameplay, challenges,
experiments, and themed content packages. They let DIY-AI change rules and
expose prepackaged client content without requiring a new player build, while
keeping ordinary decks, matchmaking, and balance statistics clean.

The framework must support all of these with one model:

- ordinary declarative deck-limit changes;
- precise per-card availability and copy limits;
- named hidden, cross-faction, additive, restrictive, and excluded card pools;
- complex deterministic deck-building logic implemented by a rule card;
- runtime `CardEffect` behavior in the rule zone;
- server-authored AI and special modes;
- theme or expansion packages;
- isolated player testing followed by promotion of the same stable `CardId`;
- immediate server-side exposure changes and rollback.

## Non-negotiable boundaries

- The server is authoritative for deck legality and matchmaking.
- Rule-card deck order never determines behavior.
- A rule evaluation is deterministic and has no random, time, database-write,
  network, or live-game-state dependency.
- Existing rule decks are hidden or disabled when exposure closes; they are not
  deleted or silently rewritten.
- Runtime rule support is independent from the player deck-building entry.
- A server-controlled AI may use a hidden rule package. An ordinary player deck
  may enter that challenge through an explicit password and inspect the rule
  zone in game.
- Unknown card IDs, missing leaders, duplicate rule cards, invalid protocol
  revisions, and resolution-budget limits are system invariants. Rule priority
  cannot override them.
- A new server effect or a pool of card art already packaged in the client does
  not require a client release. New art, animation, shader, prefab, compiled UI,
  or protocol shape still crosses the client-release boundary.

## Rule package identity and lifecycle

Every rule definition has:

- stable rule-card `Id`;
- `PackageVersion` included in the rule fingerprint and result records;
- integer `Priority`;
- `OverrideScopes` for intentional lower-priority takeover;
- dependency and exclusion declarations;
- enabled state for runtime availability;
- independent player-selectable state;
- named card-pool references and declarative constraints;
- optional deterministic deck-building handler;
- optional normal runtime `CardEffect` handlers.

Exposure has two gates:

1. `PlayerRuleCardsEnabled` globally hides every player-facing rule-card entry
   and rule deck while leaving runtime packages active.
2. `PlayerSelectable` controls one package, enabling a staged trial or an
   immediate package-specific kill switch.

Disabling either gate never deletes a deck. Reopening the gate restores it if
the package version remains compatible.

## Deck-building adjustment event

A rule card may implement a pure out-of-game handler for
`OnDeckBuildingAdjust`. The effect class can also inherit normal `CardEffect`,
but the deck-building handler does not receive or construct a
`GwentServerGame`, `GameCard`, event pipeline, player transport, or database
session.

Input is an immutable snapshot:

- request revision and last accepted projection fingerprint;
- leader ID;
- ordered selected card IDs, including rule cards;
- candidate user action when applicable;
- enabled manifest, named pools, and package versions;
- client feature level and locale only when required for presentation.

Output is a proposal tagged with source rule ID, priority, and reason code:

- deck-size minimum and maximum;
- gold, silver, and copper total limits;
- gold, silver, and copper same-name limits;
- named pool additions, restrictions, and exclusions;
- per-card selectable state and maximum selectable quantity;
- validation issues;
- deterministic normalization/removal proposals.

Handlers cannot mutate the input deck or persist data.

## Priority and merge semantics

Rules execute in ascending `(Priority, SortOrder, CardId)` order so a higher
priority observes the lower-priority projection. The merge engine, not handler
side effects, decides the final result.

- Non-conflicting proposals coexist.
- For the same constraint ID or per-card property, the highest priority wins.
- Equal-priority incompatible claims produce a visible conflict; CardId order
  is not used as a silent winner.
- A high-priority rule may claim an override scope. This suppresses every
  lower-priority rule proposal in that scope, including distinct constraint
  IDs. It does not suppress base constraints unless it explicitly removes or
  replaces them.
- Supported scopes are `deck-limits`, `group-limits`, `card-limits`,
  `card-pool`, `leader`, `normalization`, and `*`.
- `*` is an explicit full takeover, not a default behavior.
- Within an unsuppressed card-pool composition: begin with the leader faction
  plus neutral base pool, union additions, intersect each rule's restriction
  group, then apply exclusions. Exclusion wins.
- For unkeyed compatible bounds, the effective lower bound is the largest
  minimum and the effective upper bound is the smallest maximum. A lower bound
  above the upper bound is a conflict.

Zero-card decks and empty pools are valid first-class results. Rule cards do
not count toward ordinary deck size. A rule may replace the ordinary deck-size
constraint with `0..0` and restrict the selectable pool to an explicitly empty
named pool. It may then use its normal runtime `OnGameStart` event to populate
the game deck (for example, 40 distinct random gold cards). Runtime generation
uses the match RNG and persists the seed/package version; insufficient
candidates follow an explicit rule-defined failure or fallback policy and can
never silently repeat cards or loop.

Deck-building completeness and runtime readiness are deliberately separate:

- a `0..0` ordinary deck is complete when the selected rules say it is;
- an explicitly empty pool means no ordinary card is selectable, not “fall
  back to the base pool”;
- match creation must preserve the rule cards and allow `OnGameStart` to run
  before the initial ten-card draw;
- a shared rule-zone effect executes once, then resolves its actual owner
  player indexes so the same rule can affect one or both decks without double
  execution;
- distinct random population is unique by localized-independent card name,
  canonicalizes candidates before consuming the match RNG, and is atomic under
  the fail policy;
- the match result records the random seed alongside the ruleset version and
  fingerprint, allowing the generated deck and final shuffle to be reproduced.

Every final field and card state must retain its winning source rule and reason
so the UI can explain the result.

## Server projection protocol

Opening the editor requests a complete `DeckBuildingProjection`. Every card
selection or removal then submits the current immutable deck snapshot and a
monotonically increasing client revision.

The response contains:

- accepted revision;
- manifest/ruleset version and fingerprint;
- macro deck constraints;
- card-pool fingerprint;
- full per-card state for initial/rule changes, or a delta for ordinary edits;
- normalized candidate deck;
- removals with original index, rule source, and reason;
- validity/completeness and explainable issues;
- whether user confirmation is required.

The client discards a response older than its latest submitted revision. A
failed request leaves the last accepted projection visible and blocks saving or
matching until a fresh authoritative projection succeeds.

To protect the low-bandwidth server, initial and rule-changing requests may
return a full pool; ordinary add/remove requests return only changed card
states and aggregate counters when the pool fingerprint is unchanged.

## Deterministic cleanup and fixed point

Adding an unavailable card is rejected without changing the deck. A rule-card
change that makes existing cards illegal returns a preview rather than silently
deleting cards.

After confirmation, cleanup is deterministic:

1. rule cards are deduplicated;
2. higher-priority normalization proposals win by scope;
3. ordinary cards are considered in original order;
4. earlier legal copies are retained and later excess/illegal copies removed;
5. the resulting deck is evaluated again;
6. evaluation repeats until the deck and projection fingerprint stabilize.

The server permits at most eight normalization iterations. A repeated state or
failure to stabilize returns `rules.normalization-cycle`; it never loops or
persists a partial result.

## Runtime and matchmaking

At game creation, rule cards are removed from both draw decks, deduplicated into
the rule zone, annotated with which player supplied them, and subscribed to the
normal event stream. The zone is hidden when no rule card is active.

Public PVP defaults to equal rule fingerprints. Explicit password challenges
may deliberately pair different fingerprints. Match records persist both
sides' rule IDs, package versions, and the combined ruleset fingerprint.

Ranked and ordinary balance statistics exclude rule-package matches unless a
future mode explicitly opts in. Package-specific analytics are grouped by rule
ID and package version.

## Server-driven presentation primitives

The same extension boundary includes two generic in-match presentation models.
They are deliberately declarative: the server sends state and an allow-listed
style token, never executable UI, arbitrary markup, shaders, or asset URLs.

### Card markers

Every `CardStatus` may carry zero or more marker instances. A marker has a
stable definition ID, localized name and description, optional integer value,
stack/countdown display mode, owner visibility, priority, and one generic style
token. The first compatible client renders markers using packaged primitives:

- compact icon/letter badge for one marker;
- a bounded badge rail plus overflow count for several markers;
- optional border, glow, corner, or stripe accent selected from an allow-list;
- hover/tap detail containing every active marker and its value.

Built-in states such as reveal, lock, shield, resilience, and immunity remain
native fields. A new server mechanism can use a generic marker immediately and
may gain a dedicated client treatment later without changing its wire identity.
Unknown definition or style IDs fall back to a neutral badge containing a
short server-supplied label; they never hide the state.

The server owns marker mutations. IDs are deduplicated unless a definition is
explicitly multi-instance, values are bounded, and updates travel with the
normal authoritative card snapshot so movement, transform, death, reconnect,
and spectating cannot leave a stale visual marker behind.

### Player resources

Each side may expose zero or more resource values in a normally hidden resource
zone. A resource has a stable definition ID, localized label and description,
current value, optional minimum/maximum, visibility, priority, compact format,
and an allow-listed style token. One resource uses a compact pill; multiple
resources use a bounded horizontal/vertical tray with hover/tap details and an
overflow affordance. Changes may animate as generic increments/decrements.
The description is optional: when it is empty the client creates no hover/tap
target and shows no interactive cursor or empty tooltip.

Resources are ordinary game state, not deck cards. They are included in full
game information, reconnect/spectator snapshots, and a small incremental update
operation. Server helpers clamp values and send one authoritative update. An
unknown resource still renders with the neutral fallback. No resources means
the zone is absent and consumes no layout space.

Marker/resource definition catalogs are part of the hot-reloaded feature
manifest and version fingerprint. Old clients that understand the generic
protocol can display new definitions without rebuilding; clients predating the
protocol ignore the additive fields safely. New art, animation, shader, prefab,
or a new protocol shape still requires a client release.

## Theme and test packages

A theme package unlocks hidden or cross-faction cards and may add deck and
runtime behavior. Removing the rule card removes package access without
removing the card definitions from CardMap.

A test package follows this lifecycle:

1. append stable test `CardId` definitions and reuse packaged art or ship a
   planned client content update;
2. keep them out of the ordinary user pool;
3. expose them only through a versioned test rule and isolated matchmaking or
   a password AI challenge;
4. record and analyze package-version results separately;
5. use the package kill switch if needed;
6. after acceptance, add the same `CardId` to the ordinary pool and provide a
   transition preview for decks that remove the test rule.

No duplicate “test copy” of a promoted card is created.

## Reliability and acceptance gates

Automated coverage must prove:

- global and per-package exposure gates preserve hidden decks and runtime AI;
- dependency, exclusion, priority, same-priority conflict, and every override
  scope;
- additive, restrictive, excluded, hidden, and cross-faction pools;
- per-card copy limits and macro limits use the same projection on client and
  server;
- removal previews are deterministic and fixed-point cycles terminate;
- stale/out-of-order projection responses cannot overwrite current UI;
- public matching uses fingerprints while explicit password matching may not;
- rule cards never enter the draw pile and rule-zone effects receive events;
- one pathological match aborts without stopping the service or another match.

Unity acceptance must include screenshots and hands-on checks for ordinary
decks, hidden exposure, rule filters, conflicting rules, transition previews,
server-driven AI modes, password entry, rule-zone visibility, restored card
art, and leader-banner crops. A local Windows build and at least one complete AI
match are required before requesting publication approval.
