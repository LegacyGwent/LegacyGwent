# 每日登录、小王冠与 GG 奖励（diy-ai 分支）

本文描述 `diy-ai` 分支上的奖励规则与实现，是 GG 与同对手限制移植到 AI 线后的现行说明。
数值采用 AI 分支契约：登录 20，2／4／6 冠 25／35／45，GG 每次 5、每日上限 30，总额 155。

## 已实现规则

- 全球统一按中国时间（UTC+8，Asia/Shanghai）00:00 重置，使用服务器 UTC 时钟换算，不依赖主机所在时区或玩家电脑日期。
- 登录成功自动发 20 陨星粉尘；当天多次登录、刷新或断线重连不重复发放。保持在线的账号在跨日后的首次任务同步也会领取当天登录奖励。
- 真人对局每赢一个实际结算的小局得到 1 个小王冠，累计 2／4／6 冠分别额外发 25／35／45 粉尘，登录与王冠合计最多 125 粉尘。第 6 冠以后进度和奖励封顶。
- 人机练习、自我对局、平局不计每日王冠。投降／断线只保留此前已经结算赢下的小局，不凭整场胜利补发未进行的小局。匹配和密码真人房均计入。
- 王冠按服务器处理奖励时的中国日期归属，不按小局结束日期归属。零点前结束、零点后首次处理的小局计入新一天，并按新一天的进度与上限发奖；服务器提供的未来结算时间仍不接受。
- 同一账号同一小局只处理一次，跨日和重启重试也不重复计冠；当天已经封顶后处理的小局同样记入去重记录，不能通过次日重试获取奖励。服务器时钟倒退到已保存日期之前时停止发放。
- 对局结束后向对手发送 GG（Good Game）时，接收方每个有效 GG 得到 5 陨星粉尘，每天最多 6 个（30 粉尘）；第 7 个起不再增加进度或奖励，但社交 GG 提示与 `GGsReceived` 计数仍按每个有效对局一次保留。
- 每天与同一对手仅首场真人对局可获得小王冠与 GG 粉尘；当天再次遇到同一对手时，这两项奖励均不再发放。登录奖励独立于该限制，仍按当天首次登录发放。
- 首场资格适用于普通匹配和密码真人房；投降或断线的真人对局同样占用当天与该对手的首场资格。判断只读现有服务端 `GameResults` 对局记录，不新建反刷集合。
- GG 只发给服务器记录中真实完成的真人对手：发送者身份取自认证连接，接收者与场次取自服务器对局回执，客户端提交的名字不能指定身份、对手或场次；自我、人机、非参赛者、未完成对局与伪造发送者一律不发奖，且同一发送者同一场对局只处理一次。
- 每日总额 = 登录 20 + 小王冠 25／35／45 + GG 30 = 155。GG 与王冠共用“按服务器处理时的中国日期归属、跨日去重、Revision 原子提交”规则，封顶事件同样进入去重账本。

配置位于 `src/Cynthia.Card/src/Cynthia.Card.Server/DailyQuests.json`，服务器启动后缓存配置；改动后需重启服务。当前值为 `LoginPowder=20`、`Tiers=2/4/6 冠 25/35/45`、`GGPowder=5`、`GGDailyCap=30`（合计 155）。旧版缺少 GG 字段的配置会按 5／30 默认值加载。GG 单次奖励和上限必须为正数，且上限为单次奖励的整数倍；显式零或负数会被拒绝。

## 服务端与持久化

同对手规则的独立说明见 [SameOpponentRewardsVerification.md](SameOpponentRewardsVerification.md)，GG 奖励见 [GGRewardsVerification.md](GGRewardsVerification.md)。

`GwentServerGame.BigRoundEnd` 从双方实际场面得分确定胜者，通过 `GwentMatchs` 绑定的服务端回调计冠。AI 分支保持既有设计：回调只做入队，绝不阻塞小局推进。

- 真人对局创建时生成一个服务器场次 id，同时作为 `GwentServerGame` 的小局奖励 id 前缀、落库 `GameResult.Id` 和 GG 回执 id。
- `GwentMatchs` 在小局结算时调用 `GwentServerService.QueueDailyCrown(winner, opponent, matchId, roundId, settledUtc)`，把钱包归属账号、双方显示名和场次 id 一起入队。
- `RewardSettlementService` 后台顺序处理队列，先把工作项写入 `daily_round_reward_jobs`（含 `PlayerName`、`OpponentPlayerName`、`MatchId`），再调用 `GwentDatabaseService.AwardDailyCrown(账号, 显示名, 对手显示名, 场次 id, 小局 id, 结算时间)`。
- 奖励结算失败会记录 `Attempts`／`LastError` 并按 5 秒退避重试；进程重启时 `GetPendingDailyRoundRewardJobs` 会重新装载未完成工作项并保留对手／场次上下文。改动前写入的旧工作项缺少这些字段时回退到不带同对手校验的旧路径。
- `GwentMatchs` 在对局结束、结果发回客户端之前调用 `GwentServerService.RecordFinishedMatch`，仅登记两个不同账号的真人局，形成 GG 授权回执。
- `GwentHub.SendGG(MyName, EnemyName)` 保持老客户端的两字符串签名，但 `GwentServerService.SendGG` 改为用 `Context.ConnectionId` 解析真实发送者，并只接受“最近一场真实完成对局”的对手：接收者账号、场次 id 与结算时间全部来自回执，调用方名字只用于比对对手显示名，不参与授权。人机、自我、非参赛者与未完成对局没有回执，因此不会发奖。

每日日期、登录已领取标记、小王冠进度、当日已得粉尘、最多 6 个当日计冠小局键，嵌入既有 `premium_collection` 账号文档。`DailyQuests.ProcessedRoundIds` 另保存所有已处理小局键（包括封顶后的小局），跨日保留。GG 使用同一文档的 `GGReceived`（当日已奖励 0..6）、`GGPowderGranted`（0..30）与服务器专用 `ProcessedGGIds`（已处理的场次 id，含封顶场次），跨日保留且不随日期重置清空。数据库保留完整历史；服务端专用 JSON 转换器把两个 `Processed*Ids` 账本排除在 SignalR/MVC 响应之外，Newtonsoft 路径继续使用 `JsonIgnore`。任务状态、去重记录和钱包余额以同一 Mongo 原子更新提交，并以 Revision 比较交换处理并发；冲突重试会重新读取服务器日期，以成功更新那次处理所采样的日期归属。旧的 `UserInfo` 整体保存不会覆盖任务或粉尘；与闪卡合成和管理员发奖并发时不会丢失余额更新。

同对手校验在每次 CAS 尝试时用 `gameresults` 现查：按中国日界取当天、截止本场结束时间、排除本场 `Id`，红蓝双方名双向匹配。首次使用时在 `gameresults` 上建立 `RedPlayerName`／`BluePlayerName`／`Time` 复合索引 `daily_reward_players_time`，不新增集合。

旧文档缺少 `ProcessedRoundIds`／`ProcessedGGIds` 时，首次更新会先保留当前 `RoundIds`，再重置日期；旧版本已经清除的更早日期记录以及当时未保存的封顶事件无法追溯。跨日去重记录随已处理事件数持续增长，不按日清理；长期运行需另行规划归档。此改动不增加自动补发队列。

GG 发奖的持久去重账本与临时对局回执分开：回执在内存中保留每个账号最近一场真人对局、有效期 6 小时。重启会清空尚未使用的回执，重启前尚未发送的 GG 不能补领；已发奖励与去重账本仍保存在数据库中。GG 提示和社交计数属于发奖后的通知，失败不会回滚粉尘；当前没有通知／社交计数的自动重试队列。

登录在服务端主动触发当天奖励，不依赖客户端先打开任务页。小局结算持久化完成后向客户端发送 `DailyQuestsChanged`，客户端重新获取认证状态；进度没有单独的客户端发奖入口。

## AI 分支适配说明

- 保留 AI 的 `.NET 10`、`MongoDB.Driver 3.9`、`RewardSettlementService`、`PremiumDeckSelectionService`、`InitialPowderGrantService` 与初始粉尘、闪卡、菜单行为；未整体替换 `GwentServerService`／`GwentMatchs`／`GwentServerGame`。
- AI 原有 `QueueDailyCrown(User, roundId, settledUtc)` 三参数重载保留给合成工作项与既有测试；新增带对手和场次上下文的重载供真实真人局使用。
- `GwentMatchs` 的 `isAiMatch` 在开局时一次性求值，保持原 `InvokeGameOver(result, isAi || isAi, isCountMMR)` 语义。
- 隔离环境：服务 `card-diy-ai`（TCP 5010）、MongoDB `mongod-diy-ai`（回环 28021），数据写入逻辑库 `gwentdiy`／`Web`；不触碰 `card-diy`／5005／28020。

## 客户端

- 主菜单右上角每日任务入口、圆环时间进度、小时／分钟重置倒计时。
- 任务页显示每日登录、3 个完整王冠对应的 6 个小局刻度、连续进度条、3 档到账状态，以及收到 GG 的 0–6 次进度、0–30 粉尘和封顶提示；底部显示当日获得总额／155 与当前粉尘余额。奖励自动入账，页面无需点击领取。
- 任务页底部独立显示首场真人对手规则，持续可见，不会被完成、同步或错误状态覆盖。
- `DailyQuestPanel` 的 GG 区域位于王冠阶梯下方、设计画布 1600×900 之内；GG 单项与上限缺失时用 5／30 显示回退，服务端始终是权威。
- 登录、进入 Game／GamePlay、打开收藏、服务端结算通知时同步；在线每 60 秒同步一次。普通入口 5 秒节流，同一时刻合并未完成请求。
- 服务器返回当前 UTC 与下次重置 UTC；客户端用单调运行时间推进倒计时，不采用电脑日期。钱包和任务共用 Revision，旧响应不能回滚新余额。退出／切换账号清理任务缓存。

## 老客户端参考与素材

- 每日任务图标、王冠、进度条、计时图标与预览背景沿用此前从老昆特客户端提取并已导入 `Assets/Resources/DailyQuests` 的素材；GG 条目复用 `current_player_bg`、`Progression_bar_bg/fill`、`divider` 与原背景，粉尘图标沿用 `PremiumCrafting/Powder`。没有生成替代美术。
- 原客户端并不包含我们服务器的数据库实现。这里按它的任务／奖励表现移植到现有 UGUI，并扩展为登录、王冠和收到 GG 三类任务、统一粉尘奖励；不是导入其整套网络服务，也没有做原客户端运行截图的像素差验收。

## 验证状态（移植后）

- `python scripts/Verify-Localization.py`：通过。四语言、三份语言包一致，格式参数与运行时代码引用键有效；每份 `MenuLocales` 由 852 键增至 859 键（仅新增 7 个 GG／规则键）。
- 编译：`Cynthia.Card.Server`、`DailyQuestTest`、`RewardClientTest`、`GGRewardsTest`、`SameOpponentRewardsTest`、`RewardSystemTest`、`PremiumCraftingTest`、`Cynthia.Card.Server.Tests`、`Cynthia.Card.Gameplay.Tests` 在 .NET SDK 10.0.102 下退出码 0、编译错误 0。
- 2026-09-22 Codex 独立复核：隔离 Mongo `127.0.0.1:28129` 上，GG 54、同对手 38、每日任务 34、合成 21 项通过；客户端同步 30 项、旧字段兼容 2 项通过；另用仓库外临时测试验证真实后台队列的首场计冠、后续拦截、上下文落库与重启恢复，6 项通过，合计 185 项。生产服务与数据库未改动。
- 本轮未重跑真实 SignalR 对局、完整 `RewardSystemTest` 或 Unity 画面，也未打包或部署。运行数据库测试需显式将 `REWARD_TEST_MONGO_URI` 指向隔离实例。

## 前端规则要点

- 王冠刻度共 6 段（每段半个王冠），3 档奖励对应 2／4／6 冠。
- 任务页不显示、也不接受客户端提交的日期、胜场或奖励金额；一切以后端 `GetDailyQuests()` 响应为准。
