# Release preflight

Last verified: 2026-09-15

Load this reference before publishing a card batch or diagnosing repeated CI
deployments. Use the detailed card-batch checklist in `testing.md`.

## A card batch needs repeated CI deployments

- Symptom: a moderate card batch takes much longer than its code changes and
  requires multiple four-minute server CI/deploy cycles.
- Cause: the first push happened before checking active `GwentMap` text against
  locale copies, and an inferred gameplay rule was encoded as a passing test
  without explicit confirmation. Live verification then found description
  drift, while player feedback later disproved the inferred rule.
- Fix: reconcile source map, locales, card pool, and confirmed behavior before
  the first push. Rewrite a mistaken test immediately when the business rule is
  corrected.
- Prevention: distinguish evidence from assumptions in implementation notes,
  require positive and negative lifecycle tests, run shared-output test projects
  sequentially, and push only the consolidated candidate.
- Verification: both local suites, knowledge validation, and `git diff --check`
  pass before one CI run; afterward the live release SHA, `/healthz`, SignalR
  CardMap version, and representative CardMap entries match the candidate.

## Compiled snapshot and live card verification

From the candidate repository root, use the reusable read-only CLI instead of
reconstructing a temporary C# map dumper and SignalR/SSH probe for each batch:

```powershell
python -B skills/legacy-gwent-maintainer/scripts/card-batch-probe.py snapshot --cards 70038,43021,70201,70202 --output ../card-batch.json
python -B skills/legacy-gwent-maintainer/scripts/card-batch-probe.py live --snapshot ../card-batch.json
```

Supply every changed card ID, not just the example IDs. `snapshot` compiles the
production Common model, rejects unknown/duplicate IDs, and binds its JSON to
HEAD and the worktree dirty flag. Generate the release snapshot after committing
with a clean tree and keep its output outside the repository. A dirty snapshot
can aid development but is rejected by `live`.

After deployment, `live` verifies the snapshot source and active release SHA,
then compares CardMap version, 14 fields per card, and Chinese descriptions
through foreground SSH and read-only SignalR calls; the connection is closed in
`finally`. JSON `passed: true` with exit 0 is required. This checks metadata,
not gameplay behavior, client rendering, health, or isolation: retain those
separate release checks. No database mutation or deployment is performed.

The local CLI requires Python 3.10+ and the repository .NET SDK. Its remote
script runs on the deployed host's Python 3.5.3: avoid newer pathlib keyword
arguments such as `resolve(strict=True)`. Resolve without that keyword and
explicitly verify the release directory exists. Local syntax/unit tests cannot
prove remote standard-library compatibility; the foreground live check is the
acceptance gate.
