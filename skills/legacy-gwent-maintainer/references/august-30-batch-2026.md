# August 30, 2026 first card batch

Last verified: 2026-08-30

- CardMap `1.0.0.190` appends deckable Northern Realms Silver Soldier Egmond
  `70200`; never reuse an earlier ID for it. Egmond removes only the selected
  ally's positive `HealthStatus`, resets that Boost, and damages the selected
  enemy by the removed amount. A lethal hit Boosts Egmond by 1. Any Boost
  Egmond receives during its controller's turn, including that lethal-hit
  reward, repeats the ability.
- Do not suppress Egmond's own reward event to avoid recursion. `Boost` sends
  `AfterCardBoost` synchronously, so queue repeats while the ability is already
  resolving and drain the queue after the current resolution. This preserves
  legal chained repeats without recursive stack growth. A repeat with no
  legal target, zero removed Boost, or a nonlethal hit terminates naturally.
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
