# 同对手每日奖励限制移植与验收（diy-ai 分支）

本文件记录“每天只对同一对手的首场真人对局发奖”规则移植到 `diy-ai` 后的实现方式、AI 分支
适配与验收状态。总规则见 [DailyQuests.md](DailyQuests.md)，GG 授权细节见
[GGRewardsVerification.md](GGRewardsVerification.md)。

## 规则与实现

- 发放小王冠或 GG 粉尘前，复用 Mongo `gameresults` 查询双方当天的此前战绩；红蓝双方交换、
  普通匹配／密码房、投降／断线和非有效比赛均不能绕过。
- 钱包归属使用登录账号名，战绩查询使用服务端持有的双方显示名，与现有
  `RedPlayerName`／`BluePlayerName` 的存储含义一致。
- 真人对局的战绩 `Id`、GG 回执 id 和小局奖励 id 前缀使用同一个服务器场次 id，查询排除本场。
  已结束比赛的 GG 查询截止于本场结束时间，后续比赛记录不会反过来取消首场资格。
- 日期沿用“实际处理奖励时的中国日期（UTC+8）”。每次钱包 Revision 比较交换重试都重新检查
  日期和对手战绩，不把整场固定归属开局日期。跨日首次处理的旧事件仍按处理日规则结算；
  已处理的事件跨日不能再领取。
- 被拦事件仍进入现有持久化去重账本，不增加奖励进度或粉尘。登录奖励独立，GG 社交提示／计数保留。
  不新增反刷集合或奖励计数模型；在现有战绩集合建立双方玩家名与时间的复合索引
  `daily_reward_players_time`。
- 客户端每日任务页增加常驻两行说明，中／英／俄／波兰语同步到三个语言包目录。任务完成提示不会
  覆盖规则。

## AI 分支适配（关键）

AI 线通过后台队列发小局奖励，移植时必须让队列携带权威对手／场次上下文：

1. `GwentMatchs` 为真人对局生成一个场次 id，并同时用于 `GameResult.Id`、GG 回执 id 与
   `GwentServerGame` 的小局奖励前缀；对局结束时先 `RecordFinishedMatch` 再回传客户端。
2. 小局回调改为 `QueueDailyCrown(winner, opponent, matchId, roundId, settledUtc)`，仍只入队，
   不阻塞小局推进。AI 原有三参数重载保留给合成工作项与既有 `RewardSystemTest`。
3. `RewardSettlementService` 的队列工作项与 `daily_round_reward_jobs` 文档新增
   `PlayerName`／`OpponentPlayerName`／`MatchId`；重启后重新装载未完成工作项时这些字段一并恢复。
4. 结算调用 `AwardDailyCrown(账号, 显示名, 对手显示名, 场次 id, 小局 id, 结算时间)`；缺少上述
   字段的旧工作项回退到不带同对手校验的旧路径，保持向后兼容。
5. 同对手判定在每次 CAS 尝试内重新执行，因此与 AI 既有的“按处理日归属 + Revision 原子提交”
   语义一致。

## 自动检查清单（`src/Cynthia.Card/test/SameOpponentRewardsTest`）

| 覆盖点 | 断言 |
| --- | --- |
| 首场发奖 | 首场双方各得小王冠与 GG 粉尘（A：登录 20＋两冠 25＋GG 5＝50；B：20＋5＝25） |
| 双向拦阻 | 当天第二场对同一对手，双方王冠与 GG 均被拦，但被拦事件进入账本且社交仍有效 |
| 不同对手／跨日 | 换对手恢复发奖；中国零点后同一对手重新可发 |
| 回执顺序 | 仅当前回执不压制首场；第一场战绩晚到时不与第二场回执混淆 |
| 战绩形态 | 投降、断线、ranked、红蓝互换、同毫秒记录、不完整断线战绩均能正确判定 |
| 并发与重建 | 16 路并发重复只结算一次；新建数据库服务后仍拦第二场并保留账本 |
| 延迟首场 GG | 首场 GG 忽略自身已保存战绩与后续同对手战绩；第二场排除自身但仍看到首场 |
| CAS 重试 | 强制 Mongo Revision 冲突后按新的中国日期重新判定，不沿用旧日期的拦截结论 |
| 旧接口 | 三参数 `AwardDailyCrown` 仍可用且不触发同对手限制 |
| 无新集合 | 规则运行前后 Mongo 集合列表完全一致 |

## 已实际执行的验证（移植后）

- 编译：`dotnet build src/Cynthia.Card/test/SameOpponentRewardsTest/SameOpponentRewardsTest.csproj
  -p:BuildInParallel=false` → 退出码 0、编译错误 0（.NET SDK 10.0.102）。
- 编译：`GGRewardsTest`、`DailyQuestTest`、`RewardClientTest`、`RewardSystemTest`、
  `PremiumCraftingTest` 与服务端项目均退出码 0、编译错误 0。
- 语言检查：`python scripts/Verify-Localization.py` → 通过；规则说明键
  `DailyQuest_OpponentRules` 在四语言、三份语言包中一致。

## 独立运行复核（2026-09-22）

- Codex 在独立 Mongo `127.0.0.1:28129` 上运行 `SameOpponentRewardsTest`，38 项全部通过，含 CAS 冲突、并发、跨日和显示名不同于账号名。
- 同时运行 GG 54 项、每日任务 34 项、合成 21 项、客户端同步 30 项和旧字段兼容 2 项，全部通过。
- 仓库外临时测试实际启动 `RewardSettlementService` 后台服务，验证首场计冠、后续同对手拦截、三个上下文字段落库和服务重启恢复，6 项通过。本轮独立检查合计 185 项。

## 尚未重跑

- 完整 `RewardSystemTest` 与 Unity 画面。
- 真实联网：不同账号与不同显示名，经真实认证、匹配、投降结算、战绩落库、旧签名 `SendGG`、
  通知与钱包查询，验证首场发奖、第二场同对手拦截、换对手恢复。
- 测试库默认 `mongodb://127.0.0.1:28121`，可用 `REWARD_TEST_MONGO_URI` 覆盖；只在隔离服务
  `card-diy-ai`（5010）与隔离 Mongo（28021／复核指定端口）上执行，不得触碰 `card-diy`／5005／28020。

## 复现说明

- 本机 .NET SDK 10.0.102；独立构建使用 `dotnet build <项目路径> -m:1`，受限执行环境可使用 `-p:BuildInParallel=false` 避免并行构建问题。
- 新持久化测试项目：`src/Cynthia.Card/test/GGRewardsTest/`、
  `src/Cynthia.Card/test/SameOpponentRewardsTest/`。
- 运行测试需从包含 `src/Cynthia.Card/src/Cynthia.Card.Server/Locales` 的仓库内执行；测试自身
  会把工作目录切换到该服务端目录。测试使用随机前缀数据，不删除现有账号或战绩。

## 边界

- 本轮启动了独立测试 Mongo 和进程内后台队列，未启动或改动生产服务，未重跑真实联网或 Unity 画面。
- 联网对局若采用投降结束，可验证真实匹配、战绩落库与 GG 授权，但不等于人工打完整场三小局。
- 没有部署线上服务器或生成发布客户端。
