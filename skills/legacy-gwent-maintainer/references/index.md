# Knowledge index

Last verified: 2026-08-06

Read this file first, then load only the rows relevant to the task.

| Task signal | Reference | Scope |
| --- | --- | --- |
| Components, source ownership, runtime flow | [architecture.md](architecture.md) | Repository layout and component boundaries |
| Accounts, cards, decks, AI, matchmaking | [business-rules.md](business-rules.md) | Verified gameplay and domain behavior |
| Card identity, selected DIY effects, balance variants, generate rules | [card-rules.md](card-rules.md) | Verified card-specific behavior |
| August 5-6 Monster card batches | [monster-batches-august-2026.md](monster-batches-august-2026.md) | Restored pool, effects, and edge conditions |
| August 6 Nilfgaard/global card batch | [nilfgaard-batch-august-2026.md](nilfgaard-batch-august-2026.md) | Restored pool, effects, rounding rule, and regressions |
| Deployment, landing, damage, shield, duel, repeated effects | [gameplay-lifecycle.md](gameplay-lifecycle.md) | Server gameplay pipeline and timing boundaries |
| Complex card effects, headless matches, deterministic scenarios | [testing.md](testing.md) | Isolated in-process gameplay tests and fixture |
| Rule cards, mode menu, deck editor and generic HUD acceptance | [rule-framework-ux.md](rule-framework-ux.md) | Durable architecture and visual QA checklist |
| Card-batch publication, preflight, repeated CI | [release-preflight.md](release-preflight.md) | First-push gates and release-loop prevention |
| Card-pool reset, retirement, deck-code compatibility | [card-pool-migrations.md](card-pool-migrations.md) | Immutable map ordering, classification, data migration |
| Windows setup, local server, MongoDB, Unity | [development.md](development.md) | Reproducible local workflows |
| Linux server, SSH, systemd, CI/CD, rollback | [operations.md](operations.md) | Stable and DIY-AI operations |
| Runtime packages, vulnerability audits, upgrade paths | [dependencies.md](dependencies.md) | Supported pins and phased security upgrades |
| Unity startup, runtime localization, platform launch | [unity-pitfalls.md](unity-pitfalls.md) | Unity runtime root causes and verified remedies |
| Unity CI, packaging, versions, release artifacts | [unity-release-pitfalls.md](unity-release-pitfalls.md) | Unity release root causes and verified remedies |
| Branches, worktrees, PR integration, workflow triggers | [integration-pitfalls.md](integration-pitfalls.md) | Integration drift and automation traps |
| Other failure, confusing symptom, known trap | [pitfalls.md](pitfalls.md) | Cross-cutting root causes and verified remedies |
| Website, Blazor, local preview failure | [website-pitfalls.md](website-pitfalls.md) | Website-specific symptoms and verified remedies |
| Why the current approach was selected | [decisions.md](decisions.md) | Active architectural decisions |

Maintenance rule: rewrite a canonical reference when knowledge changes. Split a
file at 200 lines or 16 KiB and add each replacement directly to this table.
