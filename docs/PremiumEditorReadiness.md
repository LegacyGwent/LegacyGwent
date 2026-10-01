# Unity 编辑器闪卡加载与验收

编辑器能进入登录或收藏界面，不代表闪卡资源已经可播放。开发时要同时检查
资源来源、闪卡画质、卡片的普通/闪卡版本，并观察动画连续播放。

## 为什么修改资源后会再次变成静态卡

闪卡源资源在 `Assets/DynamicCards/Content`，编辑器资源包在
`Library/DynamicCardsBundles/StandaloneWindows64`。两者不是同一份内容。
源资源或导入设置变化后，缓存校验会移除 `cards.bundle.editor-ready`；这是正常的失效保护，
不能手动补写这个文件来冒充缓存已更新。

过去，源资源加载默认关闭。缓存失效后，即使完整源资源已导入，主游戏也只显示静态画。
临时打开源预览做验收、结束后恢复关闭，会使同一问题再次出现。

现在交互式编辑器默认使用 **Automatic**：优先使用通过检查的资源包；缓存不可用且
源资源完整时，自动从源资源预览。退出并重新进入 Play Mode 会重新检查，
不需要依赖某个开发者临时打开的旧开关。

## 开发者入口

打开 **Tools → Dynamic Cards → Preview Status**。

| 模式 | 使用方式 |
| --- | --- |
| `Automatic` | 默认开发模式。有效缓存优先，否则使用完整源资源。 |
| `BundlesOnly` | 只使用资源包，供加载性能和缓存交付验收；资源包不可用时显示原因。 |
| `SourceOnly` | 强制源资源预览，用于修改模型、材质和动画后的对照。 |

模式是当前工程的编辑器设置，不写入玩家安装包。旧测试工具恢复
`AllowEditorSourceLoading=false` 时回到 `Automatic`，不会再次禁用自动预览。
批处理中的 `Automatic` 等价于 `BundlesOnly`；源预览测试必须明确选择 `SourceOnly`。

源预览首次加载可能卡顿，Unity 2019 的 AssetDatabase 读取是同步的。
它沿用可见卡片的串行加载队列，不预加载全部卡片；性能测试请构建资源包并选择 `BundlesOnly`。

### 状态正常但卡仍然不动

1. 在游戏设置中检查「闪卡画质」。明确选择的「关闭」会被保留，不会被资源修复强制打开。
   画质设置和预览资源来源是两项独立设置，详见[闪卡画质档位](DynamicCardQuality.md)。
2. 确认正在预览闪卡版本；普通卡本来就是静态的。不要通过修改账号所有权解决资源加载问题。
3. 在状态窗口检查目录、缺失源资源数和实际选择的来源；到 Console 查看具体原因。
4. 如果整个目录未恢复，按仓库现有的源资源流程运行
   `python scripts/premium-content.py restore`，然后等待 Unity 完成导入。
   此命令从仓库根目录执行，并按既有清洁目录和哈希校验规则工作；不要覆盖已有修改的源资源。
5. 需要资源包时，停止 Play Mode，通过
   **Tools → Dynamic Cards → Build Options → 仅构建动态卡资源包** 重建，
   再重新进入 Play Mode。重建完成前不要伪造 ready 标记。

状态检查验证 catalog、索引版本、卡片映射及分包文件，不对每次启动的全部纹理重新做大体积哈希。
缓存失效仍由原有资源后处理校验负责；文件存在检查也不等于所有卡片均已完成运行验收。

## 提交前必须验证的行为

- `Automatic` 在真实的缺失/过期缓存状态下，能显示当前源资源动画。
- 停止并重新进入 Play Mode，仍能播放，不能只验证一次临时开关状态。
- `BundlesOnly` 对缺失或异常缓存给出明确诊断，不把源预览冒充资源包验收。
- 用户明确选择画质「关闭」后保持关闭；普通卡的展示语义保持不变。
- 在主游戏收藏页预览至少一张有骨骼动画和一张有效果动画的闪卡，连续多帧验证画面变化。

编辑器数据回归入口为 **Tools → Dynamic Cards → Verify Editor Content Policy**。
也可向仓库 `work/PremiumEditorReadiness20261002/command.json` 写入
`{"action":"test"}`；结果在同目录 `tests.json`。`{"action":"status"}` 则写出当前 `status.json`。
主游戏回归工具 `DynamicCardPlayModeVerification` 走真实 `EditorInfo` / `DynamicCardView`
预览路径，记录画面变化和模型状态，不把单次截图当作动画通过。
在已登录本地测试账号的主游戏中，向同目录 `ui-request.json` 写入
`{"action":"capture","run":"first-play","cards":["14003","12011"]}`。
结果、每张卡的连续三帧和 Game 截图保存在 `first-play` 子目录。停止并重新
进入游戏后，用新的 `run` 名称复测。此工具要求真实连接 loopback 5005；
不要把它用于线上账号或通过修改所有权来通过检查。
请求中的卡必须是该测试账号已拥有的闪卡；`inspect` 的
`state.ownedPremiumCards` 可用于选择样本。未拥有时会明确阻止测试。

编辑器源预览通过只说明开发环境可播放。发布 premium 客户端仍须执行既有
`LegacyClientBuild.Build` 资源构建、安装包资源检查和目标平台实际运行验收。
standard 安装包不会因本编辑器功能获得 premium 资源能力。

## 本地游戏验证

先核实正在运行的本地服务器、数据库目录和客户端实际 TCP 连接。
原有 `work/LocalServer` 的 5005/28020 与新 AI 开发环境的 5010/28021 是不同的环境；
不要因为其中一个数据库为空就认定旧测试账号不存在。
本地账号凭据不应写入新的诊断日志、提交或上传的验收结果。
