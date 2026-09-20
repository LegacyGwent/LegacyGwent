# Integration pitfalls

Last verified: 2026-09-21

## A stable-DIY commit compiles but is unsafe to cherry-pick into DIY-AI

- Symptom: a season, spectator, or UI commit from `diy` appears mergeable into
  `diy-ai`, yet it can overwrite an active season, change serialized enum
  values, reintroduce stale card text, or remove inventory still referenced by
  player data.
- Cause: the branches share source history but have independent Mongo state,
  CardMap/locales, protocol consumers, and later card fixes. A green historical
  build proves compilation at its old base, not compatibility with the current
  AI branch or database.
- Fix: diff immutable branch heads from their merge base, classify individual
  hunks, and port only the final intended behavior. Keep new protocol enum
  members at the end, require unique season IDs, preserve current AI card rules,
  and audit stateful removals against Mongo 28021 before changing them.
- Prevention: never cherry-pick a post-split DIY commit wholesale. Package a
  direct source-only fix separately from protocol, season-data, and Unity scene
  work; document unresolved product choices before implementation.
- Verification: run current server/gameplay tests, inspect numeric enum values
  and season-ID uniqueness, compare all locale surfaces, exercise the complete
  client/server path for protocol changes, and preview any 28021 migration with
  a restorable backup plan.

## A feature port leaves generic UI translations behind

- Symptom: premium-specific text is translated but titles, borders, settings,
  and card category names revert to old English strings in the AI client.
- Cause: comparing file presence or a feature-key subset does not cover all
  personal changes inside shared language files. Whole-file copying is also
  unsafe because AI card descriptions and choice text follow newer rules.
- Fix: compare personal-base, personal-current, AI-base and AI-current values
  per UI key. Restore nonconflicting generic text without replacing AI rule
  descriptions, removed options or the AI endpoint welcome text. Synchronize
  Resources, StreamingFile and server Locales together.
- Prevention: audit original dirty files as well as committed source paths;
  distinguish source Release delivery from Git tracking and historical evidence.
- Verification: the final delivery audit restored 74 omitted UI translations;
  `scripts/Verify-Localization.py` passed across all four languages and stores.
  File/key presence alone does not establish semantic parity or device behavior.

## A clean release branch does not mean the fork has no private history

- Symptom: an AI release branch passes a text scan, but an older public branch
  still exposes workstation paths or private contact addresses in Git metadata.
- Cause: source-only ports can omit debug files and ancestry; older branches,
  release tags and author/committer fields remain independently accessible.
- Fix: inspect every relevant public ref, new-commit identity, tracked data and
  source archives. Keep findings outside the repository and never print matched
  secrets or private contact values. Prepare historical rewrites separately and
  coordinate approval before force-pushing or moving published source tags.
  If the user excludes history rewriting, make an ordinary cleanup commit on
  the affected branch instead. Redact specific paths/addresses while retaining
  diagnostic files; preserve an unrelated dirty checkout using a temporary Git
  index. Explicitly report that historical blobs and email metadata remain.
  When rewriting a not-yet-merged PR branch is authorized, sanitize every
  contributed snapshot and identity while preserving the upstream base. Check
  published tags separately and retain source Release assets when retargeting
  an affected tag. Use explicit expected-SHA leases for only the affected refs.
- Prevention: use the account's GitHub noreply identity, relative paths, and
  `scripts/check-public-content.py` for new text. Keep third-party credits.
  The guard does not scan all binary metadata or guarantee anonymity.
- Verification: source-archive scanning, Git author/committer checks and branch
  inventory are separate evidence. A new cleanup commit cannot erase old blobs,
  downloaded copies or GitHub's ownership/activity records. Copyright statements
  do not establish permission; see `ASSET_NOTICE.md` and the privacy guide.
  Zero visible forks does not prove zero clones. A rewrite can remove old
  commits from branch ancestry without removing GitHub cached commit views;
  complete server-side removal requires GitHub Support's separate assessment.

## A skill script locates the discovery alias instead of the repository

- Symptom: a script works through the physical skill path but fails with “not
  inside a Git checkout” when invoked through `.codex/skills` or another link.
- Cause: PowerShell preserves the invocation alias in `$PSScriptRoot`; joining
  parent directories does not resolve a Junction's target.
- Fix: inspect the skill-root item, resolve its `Target` when it is a link, and
  derive the repository only from that physical target.
- Prevention: keep one physical skill entity and make every discovery or
  secondary-checkout path a Junction to it. Link-aware scripts must resolve the
  skill root before looking for repository-relative files.
- Verification: run the script through the physical path, personal discovery
  path, and secondary-checkout path; all three must report the same root and SHA.

## A direct DIY-AI push immediately starts the release pipeline

- Symptom: pushing to `diy-ai` automatically queues CI and, after its gates,
  deploys to port 5010.
- Cause: the branch push is the normal release trigger; a PR is not a separate
  deployment prerequisite.
- Fix: when the user authorizes a mainline release, prepare one isolated
  candidate from current `origin/diy-ai`, finish local gates, then use
  `git push origin HEAD:refs/heads/diy-ai`. Do not add an approval requirement
  when the user has already authorized publication.
- Prevention: check the target ref, preserve protected rule-card/replay worktrees,
  and never push experimental commits to stable `diy`.
- Verification: mainline SHA, successful gated deployment, live CardMap/health,
  stable-service PID, and protected worktree fingerprints all match expectations.

## A full new Unity worktree exhausts the Windows drive

- Symptom: a new worktree fails partway through materializing duplicate art.
- Cause: the repository has large tracked Unity resources; each ordinary
  worktree needs its own copy regardless of shared Git objects.
- Fix: create the new candidate with `git worktree add --no-checkout`, configure
  a cone sparse checkout for needed source, then initialize that fresh index
  with `git read-tree -mu HEAD`. Restore only additional required art fixtures.
- Prevention: check free space first. Sparse an older task worktree only after
  confirming it is clean; never remove unrelated changes to recover space.
- Verification: the fresh candidate is clean before edits, required static art
  fixtures exist, and protected worktree fingerprints are unchanged.

## A server-only follow-up reruns every Unity desktop build

- Symptom: a PR synchronization that changes only server, website, or knowledge
  files queues Windows, macOS, and Linux Unity jobs again.
- Cause: `pull_request.paths` is evaluated against the PR's cumulative base-to-head
  diff; an earlier Unity change remains in scope on every later synchronization.
- Fix: let the final run finish, or split client and server work into separate PRs.
- Prevention: batch non-client follow-ups before the first push when one PR must
  contain both, and do not assume last-commit paths control PR workflow filters.
- Verification: compare the latest commit paths with the complete PR file list
  and the workflow event before cancelling or retriggering a queued build.

## A stale migration worktree can revert live balance rules

- Symptom: merging a long-lived migration branch silently restores an older card
  strength, formula, countdown, description, or `CardMapVersion` after the live
  DIY-AI branch has already shipped a balance patch.
- Cause: migration worktrees can remain several commits behind `origin/diy-ai`;
  resolving `GwentMap.cs` or a card-effect file wholesale with `ours`/`theirs`
  discards later per-card changes.
- Fix: refresh the migration branch before integration and resolve card-map and
  effect conflicts per card, keeping `CardMapVersion` monotonic.
- Prevention: never accept an entire conflicted card-rule file without comparing
  it with the current `origin/diy-ai`; keep a named regression test for every
  published balance batch.
- Verification: run `AugustSecondBalancePatchMatchesPublishedRules` and
  `TemporaryBalanceVariantsAreIndependentAndMatchTheirPublishedRules`, plus
  `GenerateReworkMatchesPublishedRules`, after any migration merge. Confirm the
  Viper Witcher and An Craite Greatsword Z/A/B/C parameter matrices, Spotter
  `Strength / 2`, Dimun Pirate 11, Dimun Corsair 1, Triss: Telekinesis 5, and a
  map version of at least `1.0.0.161`.
