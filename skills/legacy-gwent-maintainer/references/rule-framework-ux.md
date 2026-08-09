# Rule framework UX acceptance

Last verified: 2026-08-08

This is the durable acceptance checklist for the local-only `diy-ai` rule-card,
server-mode, generic marker, and resource framework. Do not publish the framework
until every applicable item has automated coverage or a reviewed screenshot.

## Deck editor behavior

- Opening an editor or changing the leader/rule-card selection requests one
  authoritative server projection containing the resolved constraints and card
  pool. Ordinary card add/remove operations use that snapshot locally and must
  not send one RPC per click.
- A rule card's `OnDeckBuildingAdjust` proposal is part of the authoritative
  resolved rule set, not an editor-only hint. Projection, draft/save validation,
  strict mode validation, and PVP fingerprinting must all consume the same
  effective proposal; do not rely on duplicating the effect in manifest JSON to
  make those paths accidentally agree.
- Ordinary selection must not rebuild the whole card grid, reset its scrollbar,
  flash partially loaded content, or show transient validation dialogs.
- The remaining-copy badge is only presentation state, never the authority for
  an edit. Revalidate every ordinary-card candidate locally against the accepted
  rule snapshot before mutating the deck: an event-only rule must retain the
  standard 4-gold/6-silver limits unless it explicitly replaces them. When a
  shared deck/group maximum becomes binding or is released, invalidate the
  affected visible availability in place; do not rebuild the grid or contact the
  server.
- A remaining-copy badge belongs to one card identity. Ordinary add/remove edits
  update only that card's cached badge; a global deck-size remainder must not
  make every visible card count down together. Recompute the complete snapshot
  only when the leader/rule set changes or the server returns a new projection.
- A grey/unselectable card ignores clicks locally and sends no request.
- Empty, incomplete, over-limit, retired-card, or otherwise broken decks are
  valid *drafts*: they can be saved and the player can leave the editor. They are
  merely unavailable for matching until strict server validation succeeds.
- Opening/saving an old deck never silently normalizes it. Only adding/removing a
  rule card can present one deterministic cleanup preview. Declining preserves
  the deck; accepting may leave an incomplete draft and must still allow return.
- Decide whether a rule card is selectable from the complete add-rule transition,
  including deterministic cleanup, not by validating the raw candidate deck.
  Otherwise an empty-deck or restricted-pool rule is greyed out precisely when it
  needs to offer a cleanup preview. The projection implementation must suppress
  nested card-state generation while evaluating this transition to avoid recursion.
- Count widgets show composition, not overall validity. Pool, faction, duplicate,
  retired-card, conflict, dependency, and mode checks remain separate.
- Keep the standard `current / maximum` deck-size display. When the resolved
  minimum differs from the ordinary 25-card minimum, show a separate compact
  localized `minimum N` badge; hide it for ordinary/unchanged 25-card decks.
- Rule-card entry is a fifth quality-style filter beside copper, not a global
  `rule on/off` mode. The server can hide that entry and all player rule decks;
  server-authored rule AI/password challenges still work and display their rules.
- A hard faction-scoped rule is absent outside its allowed faction, matching the
  ordinary faction card pool. Reserve grey cards for visible, potentially useful
  transitions blocked by dynamic dependencies/conflicts; do not advertise another
  faction's rule as a disabled choice.
- Adding the first rule card warns once that matchmaking conditions may change.
  Match compatibility remains server-authored mode policy and is never hard-coded
  as “identical rule cards always match.”
- Deck list defaults to mixing ordinary and rule decks, adds subtle filtering only
  when rule decks exist, and makes each deck's selected rules easy to inspect.
- `全部 / 普通卡组 / 带规则卡` is a persistent control. Switching one filter
  updates its selected state and deck entries without destroying/recreating the
  other two buttons or producing a one-frame flash.
- Rule cards remain recognizably card-shaped in both editor and match deck lists.
  Both screens use the same visual decorator: readable rule identity, a contained
  background/frame, safe right-edge padding, and a distinct treatment that cannot
  be confused with a copper special card. Avoid tiny unbacked text on wood.
- Deck-rule summaries follow the pointer while hovered and clamp/flip at screen
  edges. Never leave a tooltip fixed at the pointer-entry position or floating in
  an arbitrary central location.
- A deck row with rules keeps its compact rule summary visible both collapsed
  and expanded so the important identity is not lost when edit/delete controls
  open. Anchor the summary to the fixed painted header (not the expanding root),
  keep the adjusted title metrics, and never let generated metadata overlap the
  native buttons.

## Mode menu

- Every AI profile actually supported by the server is represented by a
  server-provided mode; the local acceptance manifest currently exposes AI0-AI5.
- Launcher and rows have obvious but size-stable hover/focus feedback: no scale,
  layout jump, or subtle color-only response. Preserve password matching.
- Keep the mode launcher in its established right-hand position immediately
  above password matchmaking. Position and render order are separate concerns:
  insert it below the card-detail view in sibling order so long descriptions
  cover the launcher without moving either control. Do not relocate it onto the
  central deck portrait merely to avoid an overlap.
- The mode chooser is a true modal at the root canvas: its dimmer and dialog sit
  above rank/avatar/name canvases and all passive match UI.
- Mode names, descriptions, availability, icons, and match policy come from the
  server. The client must not advertise an inferred rule-matching guarantee.
- Rebuild the generated mode catalogue immediately after refreshing the server
  manifest. Preserve the selected mode if it still exists; otherwise fall back
  to the server's current casual/default entry, and hide the launcher when the
  refreshed catalogue is empty. Caching the list only at scene startup defeats
  the server-side on/off control.
- Mode category headings and optional type badges are also server-authored.
  `MatchKind` selects the execution path (`pvp`/`ai`); never infer execution from
  a presentation category. This permits challenge/test/expansion categories
  without a client rebuild.
- Card-art-backed icons crop only the meaningful painted region; never shrink a
  mostly black full-card source into a small square. Use a neutral fallback icon
  when no suitable crop exists.

## In-match rule, resource, and marker UI

- Rule launcher, resource rails, and card markers are board-level HUD, comparable
  to the coin. Card details, choices, menus, and modal dialogs render above them.
  Opening the rule browser may elevate only its modal overlay, then restore the
  low launcher layer when closed.
- Hover help for resources and card markers uses a dedicated intermediate layer:
  it renders above player names and passive board labels, but below card details,
  choices, menus, and modal dialogs. Do not leave hover text on the passive HUD
  canvas where player-name UI can cover it.
- Rule launcher stays hidden when neither player has active rules. When present,
  the browser shows which side supplied each rule and supports shared rules.
- Rule cards receive the normal event stream through an explicit rule-zone
  broadcast, but ordinary card queries and target selection must exclude the
  rule zone. Never expose rule entities through a generic `GetAllCard` path just
  to make event delivery convenient; ordinary effects can otherwise select a
  rule, consume an action, and resolve as an unexplained no-op.
- Resource definitions support one or many entries and optional hover text. A
  tooltip follows the pointer while hovered and does nothing when no description
  is supplied.
- Card markers support one or many definitions/instances, readable high-contrast
  labels, stack/value display, and a polished tooltip with separated title,
  description, and localized instance details. Multiple markers remain legible.
- Resource, card-marker, and deck-rule hover cards share a restrained hierarchy:
  distinct title, divider/accent, readable wrapped body, comfortable padding,
  pointer tracking, and edge clamping. Do not render a raw debug-style text block.
- Generic HUD styles must not cover or intercept normal card/menu interaction.

## Visual regressions

- Acknowledging a completed-season reward persists per account/season. It must
  not loop, and reconnecting or logging in again must not show the same season
  dialog once more merely because the SignalR session changed.
- Instantiate canvas notification prefabs with their UI parent and
  `worldPositionStays: false`, then normalize their `RectTransform`. Passing
  world position zero can move the visible reward panel beyond the lower-left
  edge at high resolutions while its full-screen raycast backdrop still blocks
  every menu control.
- A fresh account can unlock several default cosmetics in one response. Create
  every notification inactive and advance one explicit queue from each current
  popup's confirmation button; activating every prefab immediately stacks
  several full-screen backdrops and visually duplicates titles/buttons. A
  malformed notification without a confirmation button must be discarded rather
  than blocking the remaining queue.
- The imported reward prefab hard-codes English title/button labels. Resolve the
  localization service before refreshing the authenticated account, then replace
  those labels at instantiation with the existing `NewReward` and
  `PopupWindow_OkButton` keys. Otherwise a Chinese-default build briefly falls
  back to `NEW REWARD!` / `OK` on first login even though the rest of the scene is
  localized.
- Legacy/imported accounts may contain several stale season notices. Show the
  newest pending notice once and acknowledge the complete older season backlog;
  never make a player clear one historical season on each successive login.
- Dana Meadbh's leader slot crop has no pale/white strip at the right edge, keeps
  the face on the right at the same scale as existing leader slots, and uses the
  current packaged `_slot` Addressable rather than a stale bundle. Leader slot
  art is clipped by a fixed banner mask a few pixels inside the metal frame, so
  fill-mode art cannot leak past either edge; borders and deck controls remain
  outside that mask.
- Right-click card details label navigation as `关闭`/`返回` (or localized
  equivalents), never `登录`. `关闭` exits the detail overlay; `返回` pops an
  actual linked-card browsing history and stays hidden when no prior card exists.
- User-facing server messages are localization keys resolved by the client. Do
  not concatenate Chinese and English (for example `已投降\nSurrendered`) in one
  production message.
- Archive reviewed screenshots for: ordinary empty/broken deck return, rule-card
  filter and warning, conflict cleanup, deck list with/without rules, full AI menu
  and hover, in-game hidden/visible rule launcher, rule browser, one/many resources,
  one/many card markers and tooltips, card details layering, and Dana leader slot.

## Result identity

- Combined match fingerprints include the manifest ruleset version and every
  active rule package version, not merely sorted rule IDs.
- Persist both legacy per-side rule ID lists and additive per-side versioned
  package records. This keeps old readers compatible while making historical
  challenge/test results reproducible and package analytics reliable.
