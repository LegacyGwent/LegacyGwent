# DIY-AI track

`diy-ai` is an isolated, aggressively maintained track derived from `diy`.
It is intentionally prevented from sharing the running service or database with
the stable DIY instance.

## Endpoints and isolation

| Component | DIY | DIY-AI |
| --- | --- | --- |
| Server service | `card-diy` | `card-diy-ai` |
| Public TCP port | `5005` | `5010` |
| MongoDB service | existing `mongod` | `mongod-diy-ai` |
| MongoDB port | `28020` | `28021` (loopback only) |
| Database | `gwent-diy` | `gwent-diy-ai` |
| Release directory | `/usr/share/card-diy/publish` | `/usr/share/card-diy-ai/releases/<commit>` |

Health check: `http://127.0.0.1:5010/healthz`.

To open the Unity project against the deployed AI track:

```powershell
.\scripts\open-unity.ps1 -ServerUrl http://cynthia.ovyno.com:5010
```

## CI/CD

- `diy-ai-ci.yml` builds the server image for every push and pull request that
  targets `diy-ai`, and validates shell scripts.
- `diy-ai-deploy.yml` rebuilds the same source, extracts a framework-dependent
  publish artifact, uploads it through the dedicated deploy account, and calls
  the server-side atomic deployment script.
- The deployment switches the `current` symlink, restarts only `card-diy-ai`,
  checks `/healthz`, and automatically restores the previous release on failure.
- The five newest releases are retained for manual rollback.

Required GitHub Actions secrets:

- `DIY_AI_SSH_HOST`
- `DIY_AI_SSH_PORT`
- `DIY_AI_SSH_USER`
- `DIY_AI_SSH_PRIVATE_KEY`

The deploy workflow uses the GitHub Environment named `diy-ai`. No secret is
stored in this repository.

## Server bootstrap

Run once as root on the target server:

```bash
bash deploy/diy-ai/bootstrap-server.sh
```

The bootstrap is idempotent. It creates dedicated users, directories, systemd
units, environment configuration, and the restricted deployment command. It
does not touch `card-diy`.
