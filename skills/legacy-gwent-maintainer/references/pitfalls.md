# Pitfalls

Last verified: 2026-07-31

## Unity silently connects to the public server

- Symptom: local UI works, but new accounts or results do not appear in local MongoDB.
- Cause: the client historically resolved `cynthia.ovyno.com` directly.
- Fix: configure `GWENT_SERVER_URL`; use `scripts/open-unity.ps1`.
- Prevention: keep the endpoint configurable and inspect the established TCP peer.
- Verification: Unity connects to the intended loopback or DIY-AI address.

## Registration names look reversed

- Symptom: login and display name appear swapped in MongoDB.
- Cause: form labels and stored `UserName`/`PlayerName` semantics are non-obvious.
- Fix: treat `UserName` as login and `PlayerName` as visible identity.
- Prevention: confirm the stored user document after creating test accounts.
- Verification: log in with `UserName`; observe `PlayerName` in a match result.

## AI suffix opens a waiting room

- Symptom: `ai1#` does not start an AI match.
- Cause: older server parsing recognized only `#f` while UI text documented `#`.
- Fix: normalize both suffixes in `GwentMatchs.cs`.
- Prevention: keep UI instructions and parser cases under one verification test.
- Verification: a forced match starts and persists an `aigameresults` record.

## Build fails while the local server is running

- Symptom: MSBuild cannot copy `Cynthia.Card.Server.dll` after repeated retries.
- Cause: the running .NET host locks the normal Debug output DLL on Windows.
- Fix: build with an independent output directory or stop only the local server.
- Prevention: use an isolated verification output for non-disruptive checks.
- Verification: build completes with zero errors without stopping gameplay.

## Legacy systemd returns status 127

- Symptom: valid `mongod`, `/usr/bin/test`, or `dotnet` commands exit 127 only
  when systemd starts them with `User=`/`Group=` or incompatible sandbox options.
- Cause: the old Debian 9/systemd host has a legacy account/execution-path
  incompatibility; commands work when identity is dropped through `sudo`.
- Fix: run dedicated services through `sudo -u <user> -g <group>` and omit the
  incompatible sandbox directives on this host.
- Prevention: test the exact unit on the target kernel, not only the command line.
- Verification: both dedicated services remain `active` after restart.

## New .NET process fails because ICU is absent

- Symptom: `dotnet` terminates with “Couldn't find a valid ICU package”.
- Cause: the legacy server lacks a compatible ICU package.
- Fix: set `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1`, matching existing CI.
- Prevention: keep this value in `/etc/card-diy-ai.env` and its repository example.
- Verification: `/healthz` returns 200 after a fresh service restart.

## CI trips SSH protection

- Symptom: TCP 22 opens but SSH banner, scp, or CD times out.
- Cause: `ssh-keyscan` opens several new connections and the old guard allowed
  only five new connections per source in five minutes.
- Fix: pin known host keys and use sequential scp/ssh; current guard allows normal
  development traffic while still rate-limiting bursts.
- Prevention: do not discover host keys during every deployment.
- Verification: repeated CI deployment and ordinary developer SSH both complete.
