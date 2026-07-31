# DIY-AI maintenance policy

This repository's `diy-ai` branch is the aggressively maintained AI track.

## Scope

- Start changes from `diy-ai`; never deploy experimental commits to the `diy` service.
- The isolated server is `card-diy-ai` on TCP port `5010`.
- The isolated MongoDB is `mongod-diy-ai` on loopback port `28021`, database `gwent-diy-ai`.
- Never modify, restart, migrate, or reuse the production-like `card-diy` service on port `5005` or its `gwent-diy` database.

## Required checks

- Build `src/Cynthia.Card/src/Cynthia.Card.Server/Cynthia.Card.Server.csproj`.
- Run `git diff --check` and syntax-check changed deployment scripts.
- Keep `/healthz` working; deployment depends on it for automatic rollback.
- Database changes must be backward compatible or include a tested backup and rollback path.

## Delivery

- Do not store credentials in the repository.
- Push only after CI-relevant checks pass.
- Deployments must use `deploy/diy-ai/deploy.sh`; do not overwrite the active release directory in place.
- Keep `diy` and `diy-ai` client endpoints configurable through `GWENT_SERVER_URL`.
