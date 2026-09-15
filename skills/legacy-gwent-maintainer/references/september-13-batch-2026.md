# September 13, 2026 first card batch

Last verified: 2026-09-15

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
- Ulle's September 13 revival counter was superseded by the September 14 rule:
  no Countdown, and Weaken(1) only after surviving its owner-turn-end Duel.
  Resurrection itself no longer has a counter or weakening cost.
- Ulle retains owner-turn start revival from cemetery, with a full-board
  location guard; owner-turn end chooses one match-RNG target among tied
  weakest enemies, duels it, Weakens itself by 1 after surviving and toggles its
  own lock only if still alive on the board. Existing global lock-event
  suppression remains: self-lock then death prevents automatic revival until
  an external effect unlocks/resurrects it.
- Validation: SeptemberThirteenthUlleTests now covers Duel survival/loss,
  self-Weaken to zero-base banish, full board, external revival without counters,
  lock suppression and mid-duel lock toggling. SeptemberThirteenthSmugglerTests
  covers legal/illegal/cancelled selection and actual resurrection listeners.
  DiyAiCardPoolTests checks both cards, Countdown, three locale surfaces and
  unchanged deck availability; ZoneTransitionStateTests documents A-H behavior
  without changing any shared engine, rules-card or replay implementation.
