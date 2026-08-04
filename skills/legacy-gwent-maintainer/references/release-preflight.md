# Release preflight

Last verified: 2026-08-04

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
