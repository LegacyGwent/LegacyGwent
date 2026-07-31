# Local development

The local environment uses the versions closest to production and the original
project documentation:

- ASP.NET Core Runtime 3.1.32 for the `netcoreapp3.0` server
- MongoDB 4.4.29 on `127.0.0.1:28020`
- Unity 2019.4.1f1 (`e6c045e14e4e`) for the client

Tools and database files are kept outside the repository in
`%LOCALAPPDATA%\LegacyGwentDev`.

## First-time setup

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\scripts\setup-dev.ps1
```

## Start the server

For normal development with hot reload:

```powershell
.\scripts\start-dev.ps1
```

For a background smoke-test instance:

```powershell
.\scripts\start-dev.ps1 -NoWatch -Background
```

The website is available at <http://127.0.0.1:5005/>.

For the fully isolated `diy-ai` profile (server `5010`, MongoDB `28021`, and
database `gwent-diy-ai`), use:

```powershell
.\scripts\start-ai-dev.ps1
```

## Stop

```powershell
.\scripts\stop-dev.ps1
```

Use `-KeepMongoDB` to stop only the server. MongoDB data persists between runs.

Stop the isolated AI profile with `./scripts/stop-ai-dev.ps1`.

## Unity client

Sign in to Unity Hub and activate a Unity Personal license once, then run:

```powershell
.\scripts\open-unity.ps1
```

The script rebuilds and syncs the ignored `Cynthia.Card.Common.dll` dependency
before opening the project. The client connects to the game hub on port `5005`.
