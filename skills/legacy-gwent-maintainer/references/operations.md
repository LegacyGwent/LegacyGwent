# Operations

Last verified: 2026-08-06

## Original realm

- Service `card.service` listens on port 5000 from `/usr/share/card/publish`.
- Its deployed binary uses an external MongoDB Atlas SRV connection rather
  than any of the loopback MongoDB instances. Never substitute the legacy
  `gwent` database on port 28020 when reporting current original-realm data;
  on 2026-08-03 that local collection's newest result was from 2026-05-11.
- The deployed Atlas SRV hostname no longer resolved on 2026-08-03. The service
  process and HTTP landing page remained up, but a current persisted match
  count could not be verified. Report the original-realm count as unavailable
  until database connectivity is restored; do not infer zero from the stale
  local database or HTTP health alone.

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

## Local rule-card verification

- Keep the developer manifest at
  `src/Cynthia.Card/src/Cynthia.Card.Server/Features/game-features.rule-ui.local.json`.
  Pass its absolute path when a launcher starts the server from a child working
  directory; a relative path can silently select defaults instead.
- `PlayerRuleCardsEnabled` defaults to `false`. Temporarily enable it only while
  capturing player-facing deck-builder UI, then restore `false` before handoff.
  Password-addressed AI/special modes may still expose their own match rules.
- Rule fixtures require the explicit local `-EnableRuleFixtures` switch. Never
  deploy fixture cards or use the local manifest as production configuration.
- A packaged Windows QA player may use loopback 5022 while normal local DIY-AI
  remains 5010. Stop only the exact QA profile/process afterward; never touch the
  stable DIY service or port 5005.

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

## Maintainer SSH access

- This workstation uses the dedicated Ed25519 key
  `%USERPROFILE%/.ssh/legacygwent_codex_ed25519`; its public fingerprint is
  `SHA256:iuKo+4HhU/kbYYsKZWEWjLpr3aT/rduMiWKOqAOfvoo`.
- `%USERPROFILE%/.ssh/config` maps `Host cynthia.ovyno.com` to user `root`, that
  identity file, and `IdentitiesOnly yes`. The server password was not changed.
- Verify non-interactively with
  `ssh -o BatchMode=yes root@cynthia.ovyno.com`. Never store the private key,
  password, or an SSH session transcript in the repository.
