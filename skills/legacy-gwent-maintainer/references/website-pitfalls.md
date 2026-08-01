# Website and local preview pitfalls

Last verified: 2026-08-01

## Mongo URI suffix points at the wrong apparent database

- Symptom: `gwent-diy` or `gwent-diy-ai` appears empty even though accounts and
  matches exist, or a copy of that named database leaves DIY-AI unseeded.
- Cause: `GwentDatabaseService.cs` explicitly opens `gwentdiy`, and
  `DiyPage/Command.cs` explicitly opens `Web`; the connection URI suffix is not
  used by those repositories.
- Fix: inspect, back up, and migrate both `gwentdiy` and `Web` on the intended
  Mongo port.
- Prevention: describe isolation by Mongo process/port/data directory and use
  `sync-card-diy-to-ai`, not an inferred URI database name.
- Verification: collection-count digests for both logical databases match the
  intended snapshot on ports 28020 and 28021.

## Build fails while the local server is running

- Symptom: MSBuild cannot copy `Cynthia.Card.Server.dll` after repeated retries.
- Cause: the running .NET host locks the normal Debug output DLL on Windows.
- Fix: build with an independent output directory or stop only the local server.
- Prevention: use an isolated verification output for non-disruptive checks.
- Verification: build completes with zero errors without stopping gameplay.

## A Blazor child page makes every later navigation stop responding

- Symptom: opening DIY or Card Review renders an error or empty page, then every
  link appears dead until a full reload.
- Cause: a render-time null from an empty legacy Mongo collection terminates the
  circuit; mutable static card lists also leak state across circuits. AntDesign
  drawers can additionally attempt JS interop during static prerender.
- Fix: normalize all legacy list/nested fields, keep card state per component,
  catch page reads/writes, and render drawers only after the interactive first
  render.
- Prevention: never put per-user page state in a mutable static field and never
  dereference optional Mongo documents during markup rendering.
- Verification: direct and interactive `/diy -> /vote -> /` navigation succeeds,
  an empty `Web.Admin` remains usable, and logs contain no terminated circuit or
  prerender JS-interoperability exception.

## A browser-stored username grants workshop privileges

- Symptom: changing `USER_NAME` in browser storage can reveal and execute cabinet
  actions without knowing that account's password.
- Cause: localStorage is client-controlled and was treated as authenticated
  identity.
- Fix: ignore the legacy key; restore only a time-limited server-protected
  session value and keep authorization decisions in server circuit state.
- Prevention: the owner accepts this bridge only for the function-first public
  experiment; recommend unique test credentials and do not describe it as
  production security. Before production-grade use, terminate HTTPS and replace
  the bridge with HttpOnly, Secure, SameSite cookie authentication plus
  server-side authorization on every mutation.
- Verification: login survives reload through the protected session, logout
  clears it, and an unsigned username is never accepted as identity.

## Concurrent workshop actions lose earlier votes or comments

- Symptom: one player's vote or comment disappears after another player writes.
- Cause: each Blazor circuit copied an old array and `$set` the complete Mongo
  field, so the last writer won.
- Fix: comments use `$push`; votes use conditional `$pull`/`$addToSet` and remove
  the opposite vote in the same update; adopt the returned document.
- Prevention: do not persist shared collection fields from UI snapshots.
- Verification: a local signed-in vote/comment appears in Mongo with existing
  array members preserved and the UI reflects the returned counts.

## Season totals and names disagree with game history

- Symptom: season match totals include incomplete games, `GameStatus.None` looks
  like a draw, or headings show keys such as `Season_WolfSeason`.
- Cause: raw ranked documents were counted without `IsEffective()`, opponent
  status was inferred, and locale keys were printed directly.
- Fix: use a date-bounded projection, filter `IsEffective()`, call both canonical
  player-status helpers, and resolve season/card keys through loaded game locales.
- Prevention: reuse the same validity/status rules as competitive statistics.
- Verification: the seeded three seasons contain 3,543, 17,388, and 5,101 valid
  ranked matches and show localized season names.

## Language switching returns to the previous page

- Symptom: selecting another language on `/vote` loads a previously visited
  route such as `/seasons` instead of preserving the current path and query.
- Cause: `LanguageSwitcher` lives in the reused `MainLayout`; client-side route
  changes replace the child page without guaranteeing a layout rerender, so its
  hidden `returnUrl` can retain an older `NavigationManager.Uri` value.
- Fix: immediately before native form submission, replace the hidden value with
  `window.location.pathname + window.location.search` from the live browser URL.
- Prevention: derive redirect state at the interaction boundary rather than from
  a render snapshot owned by a persistent layout.
- Verification: navigate internally to every route, switch Chinese to English
  and back, and require the exact path/query to remain while the language cookie
  and localized heading change.

## Invariant globalization hides an otherwise correct English request culture

- Symptom: browser language detection and the culture cookie work in the Windows
  preview, but the .NET 10 Linux release always renders Chinese.
- Cause: the legacy host must run with invariant globalization. The localization
  middleware still selects a named `en-US` culture, but its
  `TwoLetterISOLanguageName` is `iv`, not `en`.
- Fix: identify the website language from the full BCP-47 culture `Name`
  (`en-US`) in both `_Host.cshtml` and `SiteTextService`; keep the text service
  stateless and singleton so prerender and interactive circuits cannot share
  mutable language state.
- Prevention: never infer a named culture from derived language properties when
  the production runtime is invariant. Extend the runtime-image smoke test with
  `Accept-Language`, unsupported-language fallback, and culture-cookie priority
  assertions under the exact production globalization environment.
- Verification: English headers render `lang="en-US"` and the English hero;
  unsupported or absent headers fall back to `zh-CN`; English and Chinese
  cookies each override the opposite request header.

## Hover states make nearby controls or navigation items jump

- Symptom: a sidebar row, login button, or pager grows or changes shape on
  hover, and adjacent content shifts by a few pixels.
- Cause: hover/focus rules change transform, border width, padding, font weight,
  radius, or an auto-sized translated label.
- Fix: keep width/height, border width, padding, radius, and transform invariant
  across normal/hover/focus/active states; change only color, background,
  border color, or shadow. Give translated toolbar controls fixed geometry.
- Prevention: treat interactive-state geometry as a UI contract for dense
  navigation and toolbars.
- Verification: use a real pointer hover and compare every affected element's
  `getBoundingClientRect()` before/after; all x/y/width/height deltas must be 0.

## A stale local preview makes a completed UI fix appear broken

- Symptom: source and a new publish pass geometry checks, but the open browser
  still shows the old hover animation or layout.
- Cause: another listener/watch descendant serves an older build on the port in
  the address bar, or the tab retained old static assets.
- Fix: resolve the port's owning PID and executable path, health-check the
  intended publish, then explicitly navigate/reload the review tab.
- Prevention: record the active preview port and output path whenever a second
  preview is started; stop the whole stale watch tree when retiring it.
- Verification: the browser URL, listening PID, executable path, and tested
  build all identify the same preview.

## A generic shared folder advertises the wrong client build

- Symptom: an AITest download page appears to offer every platform, but its only
  link opens a legacy folder containing similarly versioned non-AITest files or
  no file for one of the advertised platforms.
- Cause: static platform cards and a folder-level link were not bound to the
  exact artifacts verified by CI; matching a version number was treated as
  proof of provenance.
- Fix: remove the navigation entry, home call-to-action, download route, and
  external link; return an empty `GetDownloadLink` until controlled AITest
  distribution exists.
- Prevention: restore downloads only from a manifest that records each exact
  filename, version, byte size, SHA-256, source commit, availability, and direct
  platform link. Never reuse the legacy shared folder as an AITest archive.
- Verification: the website contains no folder ID or download navigation,
  direct `/download` returns 410 Gone, `GetDownloadLink` is empty, and any future
  published file reproduces the manifest hash.

## A public website trial inherits an unauthenticated admin mutation

- Symptom: anyone who can reach 5010 can POST usernames and a trinket ID to
  `api/GwentData/AwardTrinketToUsers` and alter account cosmetics.
- Cause: the legacy controller exposed an operator script action through normal
  MVC discovery without authentication; the Unity client never uses it.
- Fix: mark the action `NonAction` on DIY-AI until an authenticated operator API
  exists. Do not change the stable branch while hardening the isolated track.
- Prevention: classify every public controller mutation as client protocol or
  operator API before deployment; operator actions must fail closed when their
  credential is absent.
- Verification: POST to the legacy route returns 404, ordinary read endpoints
  and `/hub/gwent` remain mapped, and the 2.1.9 client protocol is unchanged.
