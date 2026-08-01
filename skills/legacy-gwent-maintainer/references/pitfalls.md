# Pitfalls

Last verified: 2026-08-01

## .NET 10 build fails on a generated global using

- Symptom: the server migration fails with CS8400 in
  `Cynthia.Card.Server.GlobalUsings.g.cs` even though application code uses C# 8.
- Cause: the .NET 10 Web SDK generates a framework global using that requires
  C# 10 or newer; disabling ordinary implicit usings does not remove it.
- Fix: target C# 10 in the server project while leaving Common, AI, and Unity on
  their existing language/runtime boundary.
- Prevention: do not force the server language version below the target Web
  SDK's generated source requirements.
- Verification: a clean Release build reaches `net10.0` with zero errors, and
  Common/AI outputs remain under `netstandard2.0`.

## Long localized SignalR payloads can break the frozen Unity transport

- Symptom: Unity 2019.4's Mono `ManagedWebSocket` can close a long localized
  SignalR JSON frame with code 1007 near a fragmented UTF-8 receive boundary.
- Cause: the .NET server's `JsonHubProtocol` owns a relaxed-escaping writer;
  payload encoders or converters cannot cover protocol-owned target, invocation
  ID, and error strings. Combining payload pre-encoding with a complete-frame
  pass is redundant and expands the staged frame before the final copy,
  increasing peak allocation on large localized payloads.
- Fix: make `AsciiSafeJsonHubProtocol` the sole normalization boundary. Return
  the framework memory unchanged for all-ASCII frames; only for non-ASCII input,
  compute the exact escaped length and emit `\uXXXX` or surrogate pairs once.
  `HandshakeProtocol` error responses bypass this layer; successful handshakes
  are ASCII, but localized failed-handshake errors are not normalized.
- Prevention: do not also configure a payload encoder or string converter. Test
  through the `IHubProtocol` registered by `Startup`, including the ASCII fast
  path and protocol-envelope strings.
- Verification: write a long invocation containing non-ASCII declared names,
  dictionary keys, values, surrogate pairs, envelope fields, and completion
  error text; require ASCII output and typed round trips. Require an ASCII-only
  message to remain byte-identical to the framework protocol output.

## Windows line endings break validation or Linux host preparation

- Symptom: knowledge validation misses a visible date, or Bash reports
  unexpected EOF after a Windows-created `git archive` is extracted on Linux.
- Cause: text files can materialize as CRLF; LF-only regexes and Bash heredoc or
  control syntax then parse a different byte stream than expected.
- Fix: validators normalize CRLF before matching. For manual host preparation,
  normalize extracted scripts and unit/env files, then run target-host `bash -n`.
- Prevention: validate the extracted host payload; GitHub's Linux checkout remains LF-native.
- Verification: knowledge validators pass, every extracted script passes remote
  `bash -n`, and preparation installs byte-identical normalized files.

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

## Legacy systemd returns status 127

- Symptom: valid `mongod`, `/usr/bin/test`, or `dotnet` commands exit 127 only
  when systemd starts them with `User=`/`Group=` or incompatible sandbox options.
- Cause: the old Debian 9/systemd host has a legacy account/execution-path
  incompatibility; commands work when identity is dropped through `sudo`.
- Fix: run dedicated services through `sudo -u <user> -g <group>` and omit the
  incompatible sandbox directives on this host.
- Prevention: test the exact unit on the target kernel, not only the command line.
- Verification: both dedicated services remain `active` after restart.

## Legacy host globalization prevents the .NET 10 server from starting

- Symptom: normal globalization terminates with “Couldn't find a valid ICU
  package”; invariant mode alone instead throws `CultureNotFoundException` for
  `en-US` while old NLog initializes.
- Cause: the host has ICU 57, which .NET 10 cannot load, while NLog core 4.7.2
  still constructs an explicit `en-US` format provider that invariant mode
  rejects by default.
- Fix: set `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1` and
  `DOTNET_SYSTEM_GLOBALIZATION_PREDEFINED_CULTURES_ONLY=0`. This permits named
  cultures backed by invariant data without upgrading system ICU.
- Prevention: keep both values in `/etc/card-diy-ai.env`, its repository
  example, host-preparation script, and the runtime Docker image.
- Verification: run the self-contained release on an unused target-host port;
  `/healthz` and `/hub/gwent/negotiate` return 200, the process stops cleanly,
  and the active 5010 release remains healthy.

## CI trips SSH protection

- Symptom: TCP 22 opens but SSH banner, scp, or CD times out.
- Cause: `ssh-keyscan` opens several new connections and the old guard allowed
  only five new connections per source in five minutes.
- Fix: pin known host keys and use sequential scp/ssh; current guard allows normal
  development traffic while still rate-limiting bursts.
- Prevention: do not discover host keys during every deployment.
- Verification: repeated CI deployment and ordinary developer SSH both complete.

## A failed candidate deploys before its validation workflow finishes

- Symptom: a `diy-ai` push starts deployment while server or policy checks are
  still running, so a candidate can briefly reach 5010 even if CI later fails.
- Cause: independent push-triggered CI and deploy workflows have no dependency
  relationship.
- Fix: make deployment a reusable workflow invoked by the CI `deploy` job with
  `needs: [server, policy]`; remove its independent push trigger.
- Prevention: policy checks assert the call gate and reject a restored deploy
  push trigger. Manual dispatch remains an explicit emergency operation.
- Verification: a normal push shows deploy queued behind both CI jobs and no
  separate push-triggered deploy run exists.

## A direct DIY-AI branch push deploys without review

- Symptom: an explicitly targeted push to `diy-ai` can reach port 5010 without a
  pull request or required review.
- Cause: `diy-ai` currently has no GitHub branch protection, while its successful
  push CI invokes the reusable deployment workflow automatically.
- Fix: push candidate work to a review branch and open a PR; merge into `diy-ai`
  only after all runtime gates are complete.
- Prevention: never use `git push origin HEAD:diy-ai` as a convenience command;
  add branch protection before treating review as an enforced control.
- Verification: query branch protection and workflow triggers, then confirm the
  candidate SHA exists only on its review branch until approval.

## A server-only follow-up reruns every Unity desktop build

- Symptom: a PR synchronization that changes only server, website, or knowledge
  files queues Windows, macOS, and Linux Unity jobs again.
- Cause: `pull_request.paths` is evaluated against the PR's cumulative base-to-head
  diff; an earlier Unity change remains in scope on every later synchronization.
- Fix: let the final run finish, or split client and server work into separate PRs.
- Prevention: batch non-client follow-ups before the first push when one PR must
  contain both, and do not assume last-commit paths control PR workflow filters.
- Verification: compare the latest commit paths with the complete PR file list
  and the workflow event before cancelling or retriggering a queued build.

## `set -e` exits before a failed release can roll back

- Symptom: `current` points at a bad release after `systemctl restart` fails,
  even though the deployment script contains rollback code below it.
- Cause: a bare restart under `set -e` terminates the script before the health
  result and rollback branch can run; the same pattern can abort rollback
  verification.
- Fix: evaluate candidate and rollback restarts inside explicit conditionals,
  convert failures into unhealthy state, then execute and verify rollback.
- Prevention: every fallible command before recovery logic must be captured,
  not left as an unguarded simple command under `set -e`.
- Verification: script review and failure-path tests show both a nonzero restart
  and a failed health loop reach rollback; an unhealthy rollback stops service.

## Proxied access to port 5010 returns 403

- Symptom: direct IP `/healthz` returns 200 while the domain on port 5010 returns
  403 from a developer machine or hosted runner.
- Cause: a fake-IP/local or runner egress proxy intercepts the nonstandard HTTP
  port; the DIY-AI application itself is healthy.
- Fix: route the hostname and port directly, use the current public A record for
  development, and perform CD health verification through authenticated SSH.
- Prevention: separate application health from external proxy-path checks.
- Verification: target-host loopback and a no-proxy direct-IP request return 200.

## An ad hoc SSH loop makes ordinary commands disappear

- Symptom: tools such as `readlink` become “command not found” immediately after
  assigning a shell variable named `path` in a remote one-liner.
- Cause: root's interactive SSH command shell is zsh, where lowercase `path` is
  a special array tied to `PATH`; assigning a target pathname replaces `PATH`.
- Fix: use a neutral variable such as `target`, or execute reviewed operational
  scripts with an explicit Bash shebang.
- Prevention: never use `path` as a zsh variable in ad hoc host commands.
- Verification: the same guarded cleanup succeeds with `target` and leaves both
  DIY services active.

## PowerShell expands a remote shell substitution locally

- Symptom: an SSH command containing `$(...)` runs or fails on the Windows client
  before the intended remote install script executes.
- Cause: backslash does not escape PowerShell interpolation inside a double-quoted
  command string.
- Fix: upload a fixed reviewed shell script and execute that file remotely.
- Prevention: avoid embedding shell substitutions in PowerShell SSH strings.
- Verification: the uploaded script checksum/content is inspected, remote state
  changes as intended, and the temporary file is removed.

## A Unity cache restores another target platform

- Symptom: Linux restores a multi-gigabyte macOS `Library` cache and still spends
  a long time reimporting.
- Cause: a broad `restore-keys: Library-` prefix crosses target platforms.
- Fix: scope every restore prefix to `Library-${{ matrix.targetPlatform }}-`.
- Prevention: cache keys and fallback prefixes must include the Unity target.
- Verification: a cache miss never downloads another platform's archive.
