# Architecture

Last verified: 2026-07-31

## Main components

- ASP.NET Core server: `src/Cynthia.Card/src/Cynthia.Card.Server` targeting
  `netcoreapp3.0`.
- Shared game model: `src/Cynthia.Card/src/Cynthia.Card.Common` targeting
  `netstandard2.0`.
- Server AI: `src/Cynthia.Card/src/Cynthia.Card.AI`.
- Unity client: `src/Cynthia.Card.Unity/src/Cynthia.Unity.Card`, maintained with
  Unity 2019.4.1f1.
- MongoDB access is centralized in
  `Services/GwentGameService/GwentDatabaseService.cs`.
- SignalR gameplay hub is `/hub/gwent`; liveness endpoint is `/healthz`.

## Configuration boundaries

- Server bind address comes from `ASPNETCORE_URLS`, with 5005 as the legacy
  fallback.
- MongoDB comes from `MONGO_CONNECTION_STRING`, with the legacy local fallback
  `mongodb://localhost:28020/gwent-diy`.
- The URI selects the Mongo server, but repository code explicitly opens
  `gwentdiy` in `GwentDatabaseService.cs` and `Web` in `DiyPage/Command.cs`;
  changing only the URI database suffix does not move application data.
- Unity server selection on `diy-ai` is `GWENT_SERVER_URL`, then the tracked
  `Resources/ServerEndpoint.txt`, then a compiled direct-IP 5010 fallback.
- The shared Common DLL must be rebuilt and copied into Unity
  `Assets/Assemblies` after model changes; `scripts/open-unity.ps1` performs this.

## Branch tracks

- `diy` is the stable DIY baseline.
- `diy-ai` is the aggressive AI-maintained track derived from `diy`.
- `AGENTS.md` defines the isolation and validation policy for `diy-ai`.
