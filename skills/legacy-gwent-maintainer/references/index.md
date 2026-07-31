# Knowledge index

Last verified: 2026-08-01

Read this file first, then load only the rows relevant to the task.

| Task signal | Reference | Scope |
| --- | --- | --- |
| Components, source ownership, runtime flow | [architecture.md](architecture.md) | Repository layout and component boundaries |
| Accounts, cards, decks, AI, matchmaking | [business-rules.md](business-rules.md) | Verified gameplay and domain behavior |
| Windows setup, local server, MongoDB, Unity | [development.md](development.md) | Reproducible local workflows |
| Linux server, SSH, systemd, CI/CD, rollback | [operations.md](operations.md) | Stable and DIY-AI operations |
| Failure, confusing symptom, known trap | [pitfalls.md](pitfalls.md) | Root causes and verified remedies |
| Why the current approach was selected | [decisions.md](decisions.md) | Active architectural decisions |

Maintenance rule: rewrite a canonical reference when knowledge changes. Split a
file at 200 lines or 16 KiB and add each replacement directly to this table.
