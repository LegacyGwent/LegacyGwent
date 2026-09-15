# September 14 card batch

Last verified: 2026-09-15

CardMap `1.0.0.198` preserves the first 725 entries in order and appends Rience
`70201`, then Ramon Tyrconnel `70202`. IDs `70198` through `70200` already belong
to Girl Who Drank Brokilon Water, Endrega Queen and Egmond. Determine new IDs
from the current initialized map, never an older count in a reference.

## Changed effects

- Sigvald `70038` still counts successful self-targeted resurrection events,
  including external resurrection. Every second success Strengthens by 1;
  the previous current-power ceiling is removed.
- Vandergrift's Blade `43021` has fixed damage 10 and retains Bronze/Silver
  Cursed destruction plus banishment of its killed target before Deathwish.
  It no longer returns from the cemetery or accumulates extra damage.
- Ulle `70178` has no Countdown or resurrection-count cost. At owner turn end,
  Duel one random tied weakest enemy; only a survivor receives Weaken(1), then
  toggles Lock if still alive on the board. Losing the Duel causes neither
  Weaken nor Lock. Base-1 self-Weaken can banish Ulle before the lock operation.
  Owner-turn-start cemetery revival and global locked-event suppression remain.
- Iris: Shade `70154` is 9 power. Play initializes Countdown=2 without adding
  hand cards. Each owner turn end while on the board rechecks whether the
  opponent has passed; an eligible trigger adds one Iris' Companions to each
  hand and decrements once, with no automatic reset at zero. Replaying Shade
  starts its two activations again. Passing before the second trigger stops
  generation without consuming the remaining activation; ordinary Discard
  does not set the passed flag. The Companions' existing chosen-Discard passive
  only needs a friendly unlocked Shade on the board, independently of Truce
  and the remaining counter.
- Nenneke `43006` now uses native Resurrect to return up to three selected
  friendly Bronze/Silver cemetery Units to random deck positions. It unlocks
  and emits resurrection events without Repair or battlefield landing. Test
  Tuirseach Skirmisher and Cerys listeners rather than only final card location.
- Treant Boar `70139` deals 3 instead of 4; its other behavior is retained.
  Aguara: True Form `12030` is 1 power. Dettlaff: Higher Vampire `70002` changes
  wording only; Consume already supplies the target-power Boost, so do not
  implement that wording as an additional second Boost.

## Nilfgaard deck damage and replacement identities

- Van Moorlehem Hunter `70153` is 9 power and selects a Bronze Unit in its
  controller's deck. Philippe `70151` is 10 power and selects any Unit there.
  Damage the selected unit by ceiling half its current power, then deal its
  actual power loss to a selected battlefield unit. No enemy-only restriction
  is stated for this final selection. Armor absorption and Shield cancellation
  cannot inflate the reflected damage; account for cemetery Repair after lethal
  deck damage. Zero loss opens no board-target selection.
- Vincent `70150` stays 7 power. Randomly inspect up to three eligible enemy
  deck Units without reordering the deck or permanently revealing them. Exclude
  Gold/Leader/Special cards, current IsSpying, and intrinsic EnemyRow/EnemyPlace
  spies (their in-deck IsSpying can be false). Directly lower the chosen card's
  current power to 1, bypassing Armor/Shield, then deal the actual loss to a
  selected battlefield unit.
- Cupbearer `70152` retains its two-owner-turn counter and selects only a
  revealed Bronze hand card. Conceal it, then Heal instead of Boosting by 1.
- Rience `70201` is Nilfgaard Gold, 7 power, Mage. Select only an enemy unit,
  snapshot its current power and adjacent physical units before destruction,
  and Boost surviving non-concealed neighbors by ceiling half that power.
  Computing adjacency after immediate removal loses the correct neighbors.
- Ramon Tyrconnel `70202` is Nilfgaard Silver, 7 power, Officer. It inherits
  the prior DIY Philippe deployment/reveal behavior: 4 enemy damage on play;
  owner turn end in hand calls Reveal. A successful friendly-source reveal
  repeats damage, while null/enemy sources open no menu. Already revealed
  cards do not emit a fresh reveal event; Conceal allows another later trigger.

## Artwork and verification

- Rience uses the existing full art `c10001100`. Ramon restores the exact
  historical `d19330000` full PNG/meta and adds its Addressables entry; the
  120x173 website preview is not a replacement for the full atlas texture.
- Both original 1024x1024 textures contain their picture in the left 496x712
  region. They have no historical `_slot` PNG. `ListCardShowInfo` uses a native
  Sprite rectangle over the original pixels only for these two missing slots:
  an 8:1 496x62 strip, starting 110 pixels from the top for Rience and 86 for
  Ramon. Generated Sprite and owned Addressables handle are released on reuse
  and destruction. No original art pixels are repainted or resampled.
- A client build containing the restored art and miniature renderer is needed
  for these visuals; server CardMap hot sync cannot install new local assets.
- Behavioral coverage lives in SeptemberFourteenthIrisSupportTests,
  SeptemberFourteenthNilfgaardTests and the revised Sigvald/Blade/Ulle suites.
  Static tests verify the 14 definitions, all locale surfaces, availability,
  immutable map prefix, and full-art GUID/address membership. A passing server
  suite does not prove Unity visual or interaction acceptance.
