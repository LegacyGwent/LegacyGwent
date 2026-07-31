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
| Logical databases | `gwentdiy`, `Web` | `gwentdiy`, `Web` (separate Mongo process) |
| Release directory | `/usr/share/card-diy/publish` | `/usr/share/card-diy-ai/releases/<commit>` |

Health check: `http://127.0.0.1:5010/healthz`.

The Mongo URI suffixes `gwent-diy` and `gwent-diy-ai` do not select the
application databases. Legacy repositories explicitly open `gwentdiy` and
`Web`; isolation is provided by the different ports and data directories.

To open the Unity project against the deployed AI track:

```powershell
.\scripts\open-unity.ps1 -ServerUrl http://cynthia.ovyno.com:5010
```

If a local proxy uses fake-IP DNS and returns 403 for nonstandard ports, route
`cynthia.ovyno.com:5010` directly or pass the current public A record instead.

Packaged `diy-ai` clients use `Assets/Resources/ServerEndpoint.txt` and currently
default to the direct 5010 endpoint. Windows can still override it with
`GWENT_SERVER_URL`. The Android package ID is `cynthia.diy.ai.card`, so it can
coexist with the stable client; a build postprocessor enables the plain-HTTP
5010 connection until the service is moved behind TLS.

DIY-AI selects Chinese text and Chinese audio on first launch in both Editor and
packaged players. It stores choices under DIY-AI-specific PlayerPrefs keys, so a
stable client's previous language does not override this branch's default;
manual language changes remain persistent.

## Copy stable data into DIY-AI

Bootstrap installs `/usr/local/sbin/sync-card-diy-to-ai`. To replace only the
isolated 28021 data with an online snapshot of 28020:

```bash
sudo /usr/local/sbin/sync-card-diy-to-ai --execute
```

The command dumps `gwentdiy` and `Web`, stops only `card-diy-ai`, backs up the
old target under `/var/backups/legacy-gwent/diy-ai-sync/<run>`, restores the
snapshot, starts 5010, and rolls back automatically if restore or health checks
fail. Stable 5005 remains online, so a standalone MongoDB dump is a best-effort
online snapshot rather than a transactional point-in-time copy.

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
- `DIY_AI_SSH_KNOWN_HOSTS`

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
