# October 24 card batch

Last verified: 2026-10-25

## Movement and imported behavior

- Gezras `70207` has eight movement opportunities. Only successful `Move`
  operations consume them; an ally killed by a movement reaction still moved.
  Unused opportunities now Weaken one enemy, not an allied target.
- Bow Dryad `70114` records one pending repeat when it or an enemy actually
  moves to its ranged row during the owner's turn. It repeats at that turn's
  end, not immediately or once per moved unit. Playing/summoning to the row
  does not emit `AfterCardMove` and must not qualify.
- Do not clear pending movement at `AfterTurnStart`: an earlier listener in
  the same broadcast can already have moved an enemy. End-of-turn consumption,
  death, deployment and unlocking clear the pending state. The nested-movement
  regression is in `OctoberTwentyFourthMovementTests`.
- Xavier Moran `52003` imports DIY's maximum allied-other-Dwarf landing base
  Strength for the current round, including another same-name instance. The
  imported `AfterUnitDown` contract also observes summons/resurrections; it is
  not a new global definition of "played". Clear the maximum at round end.
  Tests must use `Effects.RaiseEvent` or real Play, not the separate legacy
  `Card.Effect` instance after recording state through global events.
- Filavandrel `51003` was already Scoia'tael in the incoming AI baseline.

## Countdown contract and Queen chains

- `SetCountdown` only sets visible state; it does not resolve reaching zero.
  `TriggerCountdown` targets one card through the ordinary dispatcher, retaining
  lock suppression. Endrega Eggs/Larva/Queen and Arachas Behemoth reuse their
  existing natural tick and resolution instead of broadcasting synthetic turn
  or Consume events. Snapshot recipients before a Queen activation so units
  generated during it do not receive an extra tick from that same activation.
- New Kikimore Queen `70213` is appended after `70212`; never reorder older
  map slots. Its natural tick is owner-turn-start, every two turns.
- Three or more copied Queens otherwise advance each other forever. The user
  approved one ability activation per physical Queen in one countdown chain.
  `CountdownTriggerChain` is explicitly carried by nested `TriggerCountdown`
  requests; a natural Queen tick starts a new chain. It is neither static
  global state nor a per-turn/card-ID restriction. Other same-name instances
  may activate, and later independent chains may activate those instances again.
- An already activated Queen still receives ticks. Reaching zero resets its
  timer to two normally, but does not propagate its ability a second time in
  that chain. Timers therefore never remain zero or run negative. Other
  insectoids continue receiving one normal tick per Queen activation.
- `OctoberTwentyFourthCountdownTests` covers three, four and six physical
  Queens, independent subsequent chains, reset without repeated propagation,
  real larva timer advancement and Behemoth spawning, plus lock/owner/zone gates.

## Resource delivery

- Otkell's `c10001000_slot.png` and Sprite importer already existed. The missing
  Miniatures Addressables entry caused deckbuilder fallback art; register the
  existing GUID/address rather than repainting the strip.
- Kikimore Queen art `d19130000` is available as a 120x173 website PNG. The
  Unity Cards and Miniatures additions copy those exact pixels and register
  Sprite assets; they are not a recovered high-resolution original.
- Source/catalog validation is not a Unity visual acceptance. Existing installed
  clients cannot acquire new local Addressables merely by downloading CardMap
  `1.0.0.212`. Art corrections require a later client/resource delivery; the
  user has deferred client packaging.

## Drain event naming

- `AfterCardDrain.Target` is the draining unit and `Source` is its victim.
  Read the constructor/caller instead of inferring direction from names.
- Plumard `70147` is deckable Copper again; keep the exact reset allowlist
  aligned. Its follow-up Drain excludes same-name drainers and victims already
  dead/off-board, avoiding a Boost from trying to drain a cemetery card.

## Source-attributed destruction

- `AfterCardDestroyed` snapshots source side/type/categories before queued
  removal. Damage, Weaken, live-unit Consume and explicit attributed destruction
  feed it. Graveyard Consume is banishment, not a fresh kill; discard and round
  cleanup have no kill attribution. Check Vampire effects across all factions:
  neutral Detlaff `70002` has a direct delayed Scorch removal too.
- A first-removal pending guard prevents nested hurt reactions from scheduling
  the same death twice and attributing it to the original nonlethal attacker.
  The real regression is Golyat at 3 power: Vampire damage 1, then Golyat's own
  damage 2. Plumard must not gain a Vampire kill. Clear the guard before death
  listeners so a resurrected physical unit can die again normally.
- `OctoberTwentyFourthPlumardTests` exercises that path, Doomed destruction,
  live hand/deck Consume, Regis deck selection, Detlaff's real `RoundPlayCard`
  pipeline, repeat death after resurrection and source transformation after
  queueing. Clear fixture starter decks when a test must select one exact card;
  use non-Token victims for resurrection (Wolf is banished on death).
