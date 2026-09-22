# GG 粉尘奖励移植与验收（diy-ai 分支）

本文件记录 GG（Good Game）奖励功能从 `diy` 线移植到 `diy-ai` 后的实现边界、AI 分支适配与
验收状态。规则说明见 [DailyQuests.md](DailyQuests.md)，同对手限制见
[SameOpponentRewardsVerification.md](SameOpponentRewardsVerification.md)。

## 功能契约

收到对手 GG 每次 5 粉尘，每个中国日最多 6 次／30 粉尘；第 7 次起不增加粉尘或任务进度，但事件仍
进入持久去重账本，社交提示与 `GGsReceived` 计数对每个有效 GG 各保留一次。原登录 20、王冠
25/35/45 不变，任务总上限 155。每场双方可各送一次。

授权只认服务器回执：`GwentHub.SendGG(MyName, EnemyName)` 保持老客户端的两字符串签名，
服务端用 `Context.ConnectionId` 解析发送者，接收者账号、场次 id、结算时间来自
`RecordFinishedMatch` 登记的最近一场真人局回执（内存、6 小时）。伪造发送者名、非参赛者、
任意对手名、自我、人机／未完成对局与过期／未来回执一律拒绝且不发奖。

## AI 分支适配

- 目标框架由源线的 `netcoreapp3.0` 改为 `net10.0`；`GGRewardsTest.csproj` 使用
  `net10.0`／`LangVersion 10.0`，与既有 AI 测试项目一致。
- `GwentServerService` 在 AI 线构造函数为 9 参数（额外含 `RewardSettlementService`、
  `PremiumDeckSelectionService`、`InitialPowderGrantService`）；测试按 AI 构造并用
  `NullLogger<T>` 提供日志，不新增依赖。
- 仓库根／服务端 `Locales` 目录改为从测试输出目录向上查找
  `src/Cynthia.Card/src/Cynthia.Card.Server/Locales/config.json`，不再依赖 `diy` 线的
  `work/GGRewards/backend-task.md` 之类被忽略的工作产物。
- 测试库默认连接 `mongodb://127.0.0.1:28121`（AI 既有约定），可用 `REWARD_TEST_MONGO_URI`
  覆盖为独立实例；不连接 `diy` 线的 28020。
- 数值断言改用 AI 契约：登录 20、三档 25/35/45、每日总上限 155；旧格式配置（缺 GG 字段）
  仍按默认 `GGPowder=5`／`GGDailyCap=30` 加载。

## 自动检查清单（`src/Cynthia.Card/test/GGRewardsTest`）

| 覆盖点 | 断言 |
| --- | --- |
| 默认契约 | 缺 GG 字段的旧配置得到 `GGPowder=5`、`GGDailyCap=30`、`DailyCap=155` |
| 发奖 | 每个有效 GG 给接收方 +5，发送方钱包不受影响，进度三项一致 |
| 封顶 | 6 次到 30，第 7 次不加进度／粉尘但仍去重，账本计数 7 |
| 并发 | 32 路同场次只结算一次；24 路唯一场次不超上限 |
| 跨日 | 中国零点重置当日 GG 计数但保留账本；封顶／已发事件跨日重放不补发 |
| 重建 | 新建 `GwentDatabaseService` 后账本仍去重、进度仍保留 |
| 旧钱包升级 | 缺少 `ProcessedGGIds` 的文档升级后保留 GG 计数与粉尘并初始化账本 |
| 序列化 | Newtonsoft 与 `DailyQuestProgressJsonConverter` 两条路径都隐藏两个 `Processed*Ids` 账本、暴露 GG 进度 |
| 服务授权 | 真实 `GwentServerService.SendGG` 拒绝未认证、无回执、伪造发送者、非参赛者、任意对手名与自我；接受真实对手；重复幂等；断线接收方照常发奖；封顶后仍社交有效 |
| 社交计数 | 首次有效 GG 计数 +1，重复不加，断线与封顶后的有效 GG 各 +1 |

## 已实际执行的验证（移植后）

- 编译：`dotnet build src/Cynthia.Card/test/GGRewardsTest/GGRewardsTest.csproj -p:BuildInParallel=false`
  → 退出码 0、编译错误 0（.NET SDK 10.0.102）。
- 语言检查：`python scripts/Verify-Localization.py` → 通过；新增 7 个键在四语言、三份语言包中一致。
- 服务端编译：`dotnet build src/Cynthia.Card/src/Cynthia.Card.Server/Cynthia.Card.Server.csproj`
  → 退出码 0、编译错误 0。

## 独立运行复核（2026-09-22）

- Codex 使用独立 Mongo `127.0.0.1:28129` 运行：`GGRewardsTest` 54/54、`SameOpponentRewardsTest` 38/38、`DailyQuestTest` 34/34、`PremiumCraftingTest` 21/21 全部通过，均使用测试账号。
- `RewardClientTest` 30/30、`PremiumCompatibilityTests` 2/2 通过；另有仓库外后台队列与重启恢复检查 6/6 通过。所有检查合计 185 项。
- 本轮独立服务端构建通过，0 编译错误；保留 NuGet 漏洞数据源暂时不可访问的 NU1900 警告。语言检查通过，12 份语言文件的原有卡牌与菜单文本均未改变。

## 尚未重跑

- 真实 SignalR 双账号密码匹配、投降结算、旧两字符串签名 `SendGG` 与 `DailyQuestsChanged`／
  `DisplayGG` 通知，验证 GG 回执授权与社交计数。
- 只在隔离服务 `card-diy-ai`（5010）与隔离 Mongo（28021／复核指定端口）上进行；不得触碰
  `card-diy`／5005／28020。

## 范围与限制

- 对局回执在内存保留每个账号最近一场、有效期 6 小时；服务器重启前尚未发送的 GG 不能补领，
  已处理奖励的持久去重仍有效。去重历史与原王冠账本一样持续增长，尚无归档机制。
- 社交计数／提示与粉尘事务分离；它们失败时记录日志但没有补发队列，粉尘不会重复或回滚。
- 本轮未重新打包客户端、未部署服务器、未重跑 Unity 画面和完整 `RewardSystemTest`。
- AI 与源线的 `diy` 服务、`diy` 客户端分支保持隔离；语言包不做整体替换，仅插入新增键。
