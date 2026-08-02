# Architecture

Last verified: 2026-08-02

## Main components

- ASP.NET Core server: `src/Cynthia.Card/src/Cynthia.Card.Server` targeting
  `net10.0` with C# 10.
- Shared game model: `src/Cynthia.Card/src/Cynthia.Card.Common` targeting
  `netstandard2.0`.
- Server AI: `src/Cynthia.Card/src/Cynthia.Card.AI`.
- Unity client: `src/Cynthia.Card.Unity/src/Cynthia.Unity.Card`, maintained with
  Unity 2019.4.1f1. Its bundled SignalR client remains 5.0.8; server framework
  upgrades must not rewrite the Unity assembly set.
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

## Website structure

- The AITest website uses `Shared/SitePage.razor`, `SiteEmptyState.razor`, and
  `RankingTable.razor` as the common subpage shell; route-specific styling stays
  in `wwwroot/css/site.css`, with workshop/drawer rules in
  `wwwroot/css/workshop-pages.css` loaded after AntDesign.
- `SiteTextService` selects exactly one language from the request culture.
  `CultureController` persists manual selection; startup uses `Accept-Language`
  for the first visit and falls back to `zh-CN`.
- DIY and review card lists belong to each Blazor component instance. `Info.cs`
  intentionally contains no mutable static page state. Mongo writes use atomic
  vote/comment operations in `DiyPage/Command.cs`.
- `SeasonOverviewService` requests a date-bounded Mongo projection, excludes
  results that fail `GameResult.IsEffective()`, uses the canonical red/blue
  status helpers, and caches the derived overview for two minutes.

## Client update boundary

- Card effect classes are discovered and executed by the authoritative server.
  The Unity client already downloads versioned card-map, trinket-map, and locale
  JSON, so compatible card rules, values, names, descriptions, and server fixes
  normally do not require a public client release.
- A new server card can reuse an existing `CardArtsId`; the Unity client then
  reuses its full art, `_slot` miniature, and voice while treating the new
  `CardId` as an independent card. Append the definition, register a distinct
  effect type, add every locale entry, and bump `CardMapVersion`; no player
  rebuild is needed when no new asset or protocol is introduced.
- Downloaded locales persist under the client data path. Card and trinket maps
  currently replace only the in-memory compiled maps; cache them atomically with
  version/hash validation and a compiled fallback before relying on them offline.
- Unity 2019.4.1f1 includes Addressables 1.18.11 and uses Addressables for card
  art, miniatures, avatars, and borders, but remote catalogs are disabled and all
  groups still use local paths. This is local packaging, not resource hot update.
- Current Addressables groups are packed together: the principal card-art bundle
  is about 172 MiB on Android and 190 MiB on Windows. Remote migration must first
  split content by change frequency or immutable pack and preserve per-platform
  `addressables_content_state.bin` files.
- Voice lines, music, and effects still load synchronously through `Resources`.
  Move these only after the art pilot, split voice downloads by language, and
  replace synchronous loads with preload/async paths.
- Weather presentation is not generic: `ShowWeather.cs` maps serialized
  `RowStatus` values to row colors with `Single`. An unknown new weather has no
  fallback and can throw in an old client; publish a generic weather presenter
  before treating future weather prefabs/materials/shaders as remote content.
  Remote shaders remain platform-specific assets: ship their materials and a
  small `ShaderVariantCollection` with the effect pack and warm it before a
  match. A new render pipeline, native plugin, or C# render controller still
  crosses the full-player boundary.
- Persistent card visuals are fixed `CardStatus` fields rendered by several
  compiled card prefabs. A new mechanic such as poison can run server-side, but
  visible stacks/duration require a bootstrap client until a generic status
  descriptor and shared status-overlay prefab exist.
- AI decisions/decks and spectator admission, visibility, ordering, and limits
  can change server-side while existing RPCs and operation DTOs remain stable.
  Separate AI menus, spectator controls/camera, and other interaction changes
  are compiled Unity UI behavior and cross the client-release boundary.
- UI/C# changes, protocol-breaking changes, engine upgrades, PlayerSettings, and
  native-plugin changes still require a full player release. Treat HybridCLR as
  a separate IL2CPP migration rather than part of the first resource-update step.

## Branch tracks

- `diy` is the stable DIY baseline.
- `diy-ai` is the aggressive AI-maintained track derived from `diy`.
- `AGENTS.md` defines the isolation and validation policy for `diy-ai`.
