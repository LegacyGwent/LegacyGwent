# September 13, 2026 first card batch

Last verified: 2026-09-13

- CardMap `1.0.0.197` keeps the existing card pool and historical order.
- Dimun Smuggler `64004`, displayed as 迪门家族走私贩, already uses the
  standard `Resurrect(MyDeck)` path. The Chinese locales already included
  “该效果视为复活”; the source GwentMap description was missing it. Align the
  source map with the locale rather than appending the sentence twice or
  replacing an already-correct effect. Bronze Unit filtering, own-cemetery
  selection and match-RNG deck insertion remain unchanged.
- Resurrection to the deck unlocks the selected unit and sends exactly one
  AfterCardResurrect, without Deploy or battlefield landing. Tuirseach
  Skirmisher gains its own 3 Strengthen there; friendly Cerys' resurrection
  counter reacts, while the opponent's does not. The existing source argument
  is the target itself and was not changed by this text clarification.
- Ulle `70178` now exposes Countdown=2 and counts only successful self-targeted
  AfterCardResurrect events, including external resurrection. Every second
  success resets the counter to 2 and applies Weaken(1), not damage; base power
  reduction survives death. A final revival at base Strength=1 can weaken to
  zero and banish the card normally. Failed revival/another card's revival
  does not consume its count.
- Ulle retains owner-turn start revival from cemetery, with a full-board
  location guard; owner-turn end chooses one match-RNG target among tied
  weakest enemies, duels it and toggles its own lock only if it survives on
  the board. It does not require winning the duel. Existing global lock-event
  suppression remains: self-lock then death prevents automatic revival until
  an external effect unlocks/resurrects it.
- Validation: SeptemberThirteenthUlleTests covers repeated real duel deaths,
  six revivals until zero-base banish, full board, external counting, lock
  suppression and mid-duel lock toggling. SeptemberThirteenthSmugglerTests
  covers legal/illegal/cancelled selection and actual resurrection listeners.
  DiyAiCardPoolTests checks both cards, Countdown, three locale surfaces and
  unchanged deck availability; ZoneTransitionStateTests documents A-H behavior
  without changing any shared engine, rules-card or replay implementation.
