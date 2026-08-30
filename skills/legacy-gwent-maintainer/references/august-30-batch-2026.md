# August 30, 2026 card batches

Last verified: 2026-08-30

- CardMap `1.0.0.190` appended deckable Northern Realms Silver Soldier Egmond
  `70200`; `1.0.0.191` raises it to 11 power and changes its repeat timing.
  Never reuse an earlier ID for it. Egmond removes only the selected ally's
  positive `HealthStatus`, resets that Boost, and damages the selected enemy by
  the removed amount. A lethal hit Boosts Egmond by 1.
- During Egmond's controller's turn, any number of Boost events targeting
  Egmond set one pending flag. The owner's `AfterTurnOver` consumes that flag
  before resolving exactly one extra ability. Suppress only flag creation while
  that end-turn repeat is resolving: its lethal reward still Boosts Egmond, but
  cannot cause a second repeat or carry into the next owner turn. Boosts during
  the opponent's turn never set the flag. Deploy still resolves once
  independently, and a resolution with no legal target or zero removed Boost
  ends safely.
- Lethal `Damage` schedules the physical cemetery move through `Game.AddTask`.
  Immediately after awaiting `Damage`, a destroyed target can still report an
  on-board row. When a same-effect reward depends on destruction, check
  `target.IsDead || !target.Status.CardRow.IsOnPlace()` rather than row state
  alone.
- Sigvald `70038`, Wraith Sorcerer `70194`, and Endrega Queen `70199` expose
  their existing two-, two-, and three-turn cadence through `CardStatus.Countdown`.
  Use `SetCountdown` so clients see every decrement and reset; do not change
  locked-card event suppression or owner-turn timing.
- Queen Adalia `70141` generates one chosen Bronze Cintra unit, then appends
  one copy of every Bronze Cintra unit to the deck in CardMap order. The old
  Northern-Realms-only starting-deck condition and random insertion positions
  are removed.
- Coën of Poviss `70158` no longer Boosts at turn start. At the end of only its
  controller's turn, it finds every tied weakest other allied Witcher and
  Boosts each by 1, then independently recalculates the current weakest set and
  repeats once. Exclude Coën itself even if its own power is the lowest.
