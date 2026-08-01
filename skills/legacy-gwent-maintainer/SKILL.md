---
name: legacy-gwent-maintainer
description: Develop, diagnose, test, deploy, and operate the LegacyGwent DIY and DIY-AI codebase while preserving durable project knowledge. Use for any LegacyGwent server, Unity client, cards, decks, matchmaking, AI behavior, MongoDB, local environment, GitHub Actions, systemd deployment, incident, or maintenance task, and whenever new business knowledge or a reusable pitfall is discovered.
---

# Legacy Gwent Maintainer

Treat repository knowledge as maintained code. Load only the references needed
for the task, then rewrite the knowledge base when verified learning occurs.

## Start every task

1. Locate the checkout and read its `AGENTS.md`.
2. Inspect the current branch, worktree, and relevant source before changing it.
3. Read [references/index.md](references/index.md) completely.
4. Read only the references routed by the index for the current task.
5. Preserve unrelated user changes and keep stable DIY resources isolated from
   DIY-AI resources.

## Work with evidence

- Prefer source code, service state, logs, MongoDB records, and successful
  end-to-end behavior over comments or UI text.
- Use repository scripts instead of reconstructing setup and deployment commands.
- Test in proportion to risk. For server changes, build the project and keep
  `/healthz` working. For gameplay changes, validate through a real local match
  when practical.
- Never commit credentials, session tokens, passwords, private keys, or raw user
  data into the skill or repository.
- Never restart, migrate, or deploy to stable `card-diy`/5005 while working on
  the isolated DIY-AI track unless the user explicitly requests it.

## Route knowledge

- Architecture or component ownership: read `references/architecture.md`.
- Gameplay, accounts, decks, AI, or matchmaking: read
  `references/business-rules.md`.
- Windows local setup or Unity: read `references/development.md`.
- Server, MongoDB, SSH, Actions, deployment, or rollback: read
  `references/operations.md`.
- Runtime/package upgrades, vulnerability findings, or dependency policy: read
  `references/dependencies.md`.
- Unity startup, packaging, versioning, or native platform launch failures: read
  `references/unity-pitfalls.md` before diagnosing.
- Unexpected behavior, failures, or repeated debugging: read
  `references/pitfalls.md` before diagnosing.
- A durable design choice or replacement of an old approach: read
  `references/decisions.md`.

## Mandatory learning rewrite

At the end of every task, explicitly decide whether verified reusable knowledge
was learned. Update the skill in the same change when any of these occurred:

- a new pitfall, root cause, diagnostic signature, or reliable fix;
- a newly understood or corrected business rule;
- a changed architecture, environment, port, service, schema, or workflow;
- a repeated command sequence that should become a repository script;
- evidence that an existing reference is wrong, stale, duplicated, or ambiguous.

Apply this rewrite procedure:

1. Select one canonical reference from the index and read it completely.
2. Merge the learning into the existing explanation. Rewrite or delete stale
   text; do not append a chronological diary entry.
3. For a pitfall, record `Symptom`, `Cause`, `Fix`, `Prevention`, and
   `Verification`. Merge duplicate symptoms under one root cause.
4. Distinguish verified facts from hypotheses. Do not store a hypothesis as
   project knowledge.
5. Update `references/index.md` when routing, scope, or file names change.
6. If a reference exceeds 200 lines or 16 KiB, split it by domain and link each
   new file directly from the index. Never create reference links more than one
   level deep.
7. Run `python scripts/validate_knowledge.py` from this skill directory and the
   skill-creator `quick_validate.py` before committing.

If nothing durable was learned, do not manufacture an update.

## Keep references compact

- Store current truth, not task transcripts.
- Prefer paths, invariants, decision rules, and verification commands.
- Remove superseded procedures when replacing them; summarize the reason in
  `references/decisions.md` only when it affects future choices.
- Never copy large source files or logs into references. Point to the source path
  and record the non-obvious conclusion.
