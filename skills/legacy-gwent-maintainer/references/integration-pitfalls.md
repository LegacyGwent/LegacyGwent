# Integration pitfalls

Last verified: 2026-08-02

## A direct DIY-AI branch push deploys without review

- Symptom: an explicitly targeted push to `diy-ai` can reach port 5010 without a
  pull request or required review.
- Cause: `diy-ai` currently has no GitHub branch protection, while its successful
  push CI invokes the reusable deployment workflow automatically.
- Fix: push candidate work to a review branch and open a PR; merge into `diy-ai`
  only after all runtime gates are complete.
- Prevention: never use `git push origin HEAD:diy-ai` as a convenience command;
  add branch protection before treating review as an enforced control. A direct
  push is reserved for an explicitly authorized DIY-AI emergency release.
- Verification: query branch protection and workflow triggers, then confirm the
  candidate SHA exists only on its review branch until approval.

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
