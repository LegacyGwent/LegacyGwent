# Operations

Last verified: 2026-07-31

## Stable DIY

- Service: `card-diy.service`.
- Public port: 5005.
- Working directory: `/usr/share/card-diy/publish`.
- MongoDB: loopback 28020, database `gwent-diy`.
- Do not restart or reuse these resources for DIY-AI work.

## DIY-AI

- Service: `card-diy-ai.service`, public port 5010.
- Mongo service: `mongod-diy-ai.service`, loopback 28021, database
  `gwent-diy-ai`, data directory `/var/lib/mongodb-diy-ai`.
- Releases: `/usr/share/card-diy-ai/releases/<commit>` with atomic `current`
  symlink switching.
- Deployment command: `/usr/local/sbin/deploy-card-diy-ai`.
- Health check: `http://127.0.0.1:5010/healthz` and public equivalent.
- Deployment keeps five releases and restores the previous symlink when health
  verification fails.

## GitHub Actions

- `DIY-AI CI` validates scripts, policy boundaries, and the server image.
- The legacy .NET workflow excludes `diy-ai` to avoid duplicate server builds.
- Desktop Unity CI runs automatically only when Unity or Common sources change;
  same-branch superseded builds are cancelled. It remains manually dispatchable.
- `DIY-AI Deploy` builds inside the pinned .NET 3.1 SDK image, uploads through
  dedicated account `card-deploy`, activates atomically, and verifies 5010 from
  the target host through the authenticated SSH channel.
- Secrets live in GitHub Environment `diy-ai`; never record their values.
- The Environment uses a custom deployment branch policy allowing only
  `diy-ai`, so other branches cannot consume its deployment credentials.
- Pin SSH host keys through `DIY_AI_SSH_KNOWN_HOSTS`; do not run `ssh-keyscan` in
  delivery because it opens enough connections to trip SSH rate limiting.

## SSH guard

- Chain `CODEX_SSH_GUARD` remains active on port 22.
- Current threshold is per source: the 30th new connection within 60 seconds is
  dropped. There is no permanent DROP for the former suspect/office IP.
- Rules are persisted through `netfilter-persistent` or `/etc/iptables/rules.v4`.
