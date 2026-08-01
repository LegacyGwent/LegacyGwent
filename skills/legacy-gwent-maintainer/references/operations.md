# Operations

Last verified: 2026-08-01

## Stable DIY

- Service: `card-diy.service`.
- Public port: 5005.
- Working directory: `/usr/share/card-diy/publish`.
- MongoDB: loopback 28020; logical databases `gwentdiy` and `Web`.
- This legacy service has no dedicated health endpoint: `/healthz` currently
  falls through to the HTML home page. Use HTTP 200 plus unchanged service PID
  and lifecycle state when proving it survived an isolated DIY-AI deployment.
- Do not restart or reuse these resources for DIY-AI work.

## DIY-AI

- Service: `card-diy-ai.service`, public port 5010.
- Mongo service: `mongod-diy-ai.service`, loopback 28021, logical databases
  `gwentdiy` and `Web`, data directory `/var/lib/mongodb-diy-ai`.
- Releases: `/usr/share/card-diy-ai/releases/<commit>` with atomic `current`
  symlink switching.
- Releases contain a self-contained `linux-x64` .NET 10 host. systemd invokes
  `/usr/local/sbin/run-card-diy-ai`, which executes `Cynthia.Card.Server`; its
  temporary `/usr/bin/dotnet` fallback keeps pre-migration releases usable for
  atomic rollback until they age out.
- Self-contained is not libc-independent. The publish requires glibc 2.27 or
  newer and `deploy.sh` rejects older hosts before switching releases. The
  current Debian 9 host reports glibc 2.28, but Debian 9 itself is outside the
  official .NET 10 support matrix.
- The host's ICU 57 is too old for .NET 10. Keep both
  `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1` and
  `DOTNET_SYSTEM_GLOBALIZATION_PREDEFINED_CULTURES_ONLY=0` in
  `/etc/card-diy-ai.env`: the second setting lets legacy NLog construct its
  `en-US` format provider while all culture data remains invariant.
- Deployment command: `/usr/local/sbin/deploy-card-diy-ai`.
- Before the first .NET 10 deployment, copy `deploy/diy-ai` from the exact
  release candidate to the host and run `prepare-net10-host.sh` as root. It
  installs the new deploy script, launcher, unit, and globalization setting
  without restarting the current service. Only then may the .NET 10 commit
  reach auto-deploy.
- Health check: `http://127.0.0.1:5010/healthz` and public equivalent.
- Deployment keeps five releases and restores the previous symlink when health
  verification or the candidate restart fails. It verifies the restored
  release too and stops the service if even rollback cannot become healthy.
- A normal service stop can cancel `ScheduledEventService` and emit a single
  `TaskCanceledException`; correlate it with systemd lifecycle timestamps.
  Error markers after the new process begins are the deployment signal.
- Stable-to-AI database copy: `/usr/local/sbin/sync-card-diy-to-ai --execute`.
  It snapshots 28020 online, stops only `card-diy-ai`, backs up 28021 under
  `/var/backups/legacy-gwent/diy-ai-sync/<run>`, replaces both logical
  databases, and rolls back on restore or health failure. Preserve the reported
  run directory until the copied state has been accepted.
- Because stable MongoDB is a standalone process and remains online, the dump is
  best-effort rather than a transactional point-in-time snapshot. Check the
  recorded source drift files and compare collection-count digests afterward.

## GitHub Actions

- `DIY-AI CI` builds the full server solution (including AITest, ConsoleTest,
  and Server.Tests) with .NET 10, executes the server compatibility tests,
  validates a self-contained Linux publish, and smoke-tests the
  `runtime-deps:10.0` image with MongoDB 4.4.
- The legacy .NET workflow excludes `diy-ai` to avoid duplicate server builds.
- Desktop Unity CI runs automatically only when Unity or Common sources change;
  same-branch superseded builds are cancelled. It remains manually dispatchable.
- `DIY-AI Deploy` uses `actions/setup-dotnet@v4` with `10.0.x`, publishes
  self-contained `linux-x64`, uploads through dedicated account `card-deploy`,
  activates atomically, and verifies 5010 from the target host through the
  authenticated SSH channel.
- A normal `diy-ai` push reaches deployment only through the `deploy` job in
  `DIY-AI CI`, after both `server` and `policy` succeed. The deploy workflow is
  reusable and manually dispatchable, but it has no independent push trigger.
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
