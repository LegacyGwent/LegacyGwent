# 投降、掉线奖励修复及验收（diy-ai 分支，2026-09-30）

## 根因和当前规则

原来只有 `BigRoundEnd` 的正常小局结算会调用王冠奖励；投降／掉线的 `GameEnd` 完全跳过这条路径，旧测试还把“不计冠”当成正确结果。

现在不同账号的真人对局中，对手投降或掉线判负，胜者按本场两枚小王冠补足：已结算一枚只补一枚，尚未结算则补两枚；败方保留此前赢得的进度。平局小局本身不发王冠，不占补足事件，因此不能用胜场数直接计算补足量。AI／同账号不发，原有同对手每日首场限制、6 冠封顶、处理日归属与粉尘档位不变，GG 仍需手动发送。

这些规则控制可申请的奖励事件；实际入账仍由原有钱包的同对手资格、当日上限及持久去重决定。跨日不会把本场此前事件重新申请一遍。

## 迁移来源与语义差异

本修复的语义来自另一个工作树（`自制闪卡测试`）上已验收的实现，源实现把奖励回调改成 `Func<int,string,DateTimeOffset,Task>` 并直接 `await` 数据库写入。AI 工作树不能整体套用该文件：

- AI 的 `GwentServerGame.RoundWon` 是 `Action<int,string,DateTimeOffset>`，只做入队（`GwentMatchs` 绑定 `GwentServerService.QueueDailyCrown`），持久化与重试在 `RewardSettlementService`／`GwentDatabaseService.AwardDailyCrown`。因此迁移**保持 Action 与入队契约不变**，把回调“返回即视为已受理”，不为等待写入而阻塞小局推进。
- `GameEnd`、`GameOverExecute`、终局闸门、投降补足、`CreateGameResult`、终局前清包、`ReceiveAsync` 终止守卫等语义按原义移植；AI 专属的匹配、卡池、AI 对手、端点与既有入队式奖励链路全部保留。
- 源实现依赖“确认丢失后用原键重试”的 `Delivered` 标志；AI 侧同样记录已分配的稳定事件，回调抛异常时按有界重试调用同一键，入队成功后由持久工作项继续幂等重试，因此不会补出第三枚。

## 最终实现

- `GwentServerGame` 用同一个 `SemaphoreSlim` 串行处理比分、回合索引、奖励事件分配和终局快照；终局只选定一次，其他结束调用等待它完成。
- 普通小局继续使用原来的 `场次ID:小局序号` 键；补足使用 `场次ID:forfeit:玩家编号:事件序号`。每枚奖励先分配稳定事件再尝试入队。入队确认丢失时重试原事件，不能再补出第三枚。
- 任一方已经自然达到两胜时，迟到的投降沿用正常胜负，不修改尚未发布的结果；不会伪造未打小局的比分。
- 奖励入队先于结束通知；逐玩家隔离消息、结果和最终数据包发送失败。只有真正的终局处理者发出完成信号，房间清理由 `GwentMatchs.StartGame` 的 `finally` 负责。
- 已结束的对局不再结算后续小局；待输入的旧游戏循环随终局退出。一般卡牌逻辑异常不伪造比分结果来抢占投降结算。
- 没有修改公共模型、客户端协议、奖励配置或数据库结构。

## 验收

DSH 负责迁移，Codex 独立审阅并重建验证。服务端使用 .NET 10；隔离宿主启用 AI 原有的后台奖励工作者，等待实际钱包结算后判断结果。

| 检查 | 结果 |
| --- | --- |
| `Cynthia.Card.Server` 隔离构建 | 0 错误；仅 NU1900 离线漏洞源警告 |
| `src/Cynthia.Card/test/SurrenderCrownTest`（新增，`net10.0`，无需数据库） | 37 项通过，0 失败 |
| 其余引用服务端的测试项目（`AITest`、`ConsoleTest`、`Cynthia.Card.Gameplay.Tests`、`Cynthia.Card.Server.Tests`、`DailyQuestTest`、`GGRewardsTest`、`PremiumCraftingTest`、`RewardClientTest`、`SameOpponentRewardsTest`、`RewardSystemTest`） | 全部构建成功，0 错误 |
| `RewardSystemTest/AdditionalScenarios.cs` 投降／掉线场景 | 已按新语义改写并编译通过；该综合套件本轮未执行 |
| Codex 独立 `SurrenderCrownTest` 重建与运行 | 37 项通过，0 失败 |
| Codex 独立生命周期、黑名单及真实 SignalR/WebSocket 验收 | 57 项通过；包含健康检查、投降、断线、同对手限制、6 冠封顶、GG 分离奖励及五场战绩 |

`SurrenderCrownTest` 覆盖 0→2、1→2、平局后补足、败方保留、真正并发的双方离开、已决定但尚未发布的自然胜负、终局等待持久化、消息失败、模糊提交重试、封顶与同对手抑制、以及终局后不再发奖。它是普通控制台程序，不经过 vstest testhost，因此可在受限沙箱内直接运行。

## 证据和复验

- `src/Cynthia.Card/test/SurrenderCrownTest/`：持久专项测试项目，不需要数据库。
- 本地独立验收产物：`AiBlacklist20260930/network-final-run.log`、`network-final-build.log`、`surrender-run.log`，以及配套 `Network` 验证源码。
- 独立宿主使用本机端口 5032、专用 Mongo 28132，没有复用 5005/5010 或 28020/28021。

## 边界

- 未部署线上服务、未重启现有常驻服务器、未生成发布客户端、未提交或推送 Git，也未更换模型配置。
- 数据库套件（`RewardSystemTest`、`DailyQuestTest`、`GGRewardsTest`、`SameOpponentRewardsTest`、`PremiumCraftingTest`）没有整套重跑；本次实际 Mongo 验证由 Codex 的隔离联网宿主完成。未改数据库结构。
- `dotnet test` 的 xunit testhost 需要命名管道，受限沙箱拒绝（`Win32Exception (5)`），因此 `Cynthia.Card.Server.Tests`／`Cynthia.Card.Gameplay.Tests` 本轮只完成构建，未执行。
- 未进行 Unity 画面和旧版客户端实机验收，也未按历史战绩追补已漏发奖励。数据库持续不可用时仍沿用有界重试并记录错误，未新增跨服务器重启的持久重试队列。
