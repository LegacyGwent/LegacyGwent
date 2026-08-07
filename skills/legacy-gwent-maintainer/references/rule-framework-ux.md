# Rule framework UX acceptance

Last verified: 2026-08-07

This is the durable acceptance checklist for the local-only `diy-ai` rule-card,
server-mode, generic marker, and resource framework. Do not publish the framework
until every applicable item has automated coverage or a reviewed screenshot.

## Deck editor behavior

- Opening an editor or changing the leader/rule-card selection requests one
  authoritative server projection containing the resolved constraints and card
  pool. Ordinary card add/remove operations use that snapshot locally and must
  not send one RPC per click.
- Ordinary selection must not rebuild the whole card grid, reset its scrollbar,
  flash partially loaded content, or show transient validation dialogs.
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

## Mode menu

- Every AI profile actually supported by the server is represented by a
  server-provided mode; the local acceptance manifest currently exposes AI0-AI5.
- Launcher and rows have obvious but size-stable hover/focus feedback: no scale,
  layout jump, or subtle color-only response. Preserve password matching.
- Mode names, descriptions, availability, icons, and match policy come from the
  server. The client must not advertise an inferred rule-matching guarantee.
- Card-art-backed icons crop only the meaningful painted region; never shrink a
  mostly black full-card source into a small square. Use a neutral fallback icon
  when no suitable crop exists.

## In-match rule, resource, and marker UI

- Rule launcher, resource rails, and card markers are board-level HUD, comparable
  to the coin. Card details, choices, menus, and modal dialogs render above them.
  Opening the rule browser may elevate only its modal overlay, then restore the
  low launcher layer when closed.
- Rule launcher stays hidden when neither player has active rules. When present,
  the browser shows which side supplied each rule and supports shared rules.
- Resource definitions support one or many entries and optional hover text. A
  tooltip follows the pointer while hovered and does nothing when no description
  is supplied.
- Card markers support one or many definitions/instances, readable high-contrast
  labels, stack/value display, and a polished tooltip with separated title,
  description, and localized instance details. Multiple markers remain legible.
- Generic HUD styles must not cover or intercept normal card/menu interaction.

## Visual regressions

- Acknowledging a completed-season reward persists per account/season. It must
  not loop, and reconnecting or logging in again must not show the same season
  dialog once more merely because the SignalR session changed.
- Legacy/imported accounts may contain several stale season notices. Show the
  newest pending notice once and acknowledge the complete older season backlog;
  never make a player clear one historical season on each successive login.
- Dana Meadbh's leader slot crop has no pale/white strip at the right edge, keeps
  the face on the right at the same scale as existing leader slots, and uses the
  current packaged `_slot` Addressable rather than a stale bundle.
- Right-click card details label navigation as `关闭`/`返回` (or localized
  equivalents), never `登录`.
- User-facing server messages are localization keys resolved by the client. Do
  not concatenate Chinese and English (for example `已投降\nSurrendered`) in one
  production message.
- Archive reviewed screenshots for: ordinary empty/broken deck return, rule-card
  filter and warning, conflict cleanup, deck list with/without rules, full AI menu
  and hover, in-game hidden/visible rule launcher, rule browser, one/many resources,
  one/many card markers and tooltips, card details layering, and Dana leader slot.
