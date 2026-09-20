# AI 分线客户端打包检查

检查日期：2026-09-20。代码基线：`1a2db7195c774fbb066e5da4ebb0aa0c6926b965`。

## 结论与证据范围

现有实现具有按平台包含或排除闪卡动画资源的机制，但当前提交尚不具备
从干净 checkout 自动交付 Windows/Android 两种内容版本的完整流程。
本次为源代码、Git 跟踪范围、资源目录和平台配置检查，未执行 Unity
Player 构建、安卓 shader 编译、APK 安装或真机测试。

| 目标 | 当前结论 |
| --- | --- |
| Windows 无动画资源版 | 已有 CI 路径和资源排除逻辑；本次没有实编译证明 |
| Windows 闪卡版 | 打包器支持，但 AI checkout 缺素材，CI 缺准备步骤及版本矩阵 |
| Android 无动画资源版 | 已有 APK CI 路径；有 ARM64 和发布签名缺口 |
| Android 闪卡版 | 同样缺素材与 CI 接线，另外缺安卓资源包、shader 和设备验收 |

这里的“无动画资源版”不是“移除所有闪卡功能的客户端”。

## 已确认的问题

### P1：闪卡素材没有进入可复现的交付链

- `Assets/DynamicCards/.gitignore` 排除 `Content/*`，只保留 catalog 及其 meta。
- AI checkout 的 catalog 有 677 个条目，677 个 prefab 路径全部不存在；
  `git ls-files Assets/DynamicCards/Content` 只返回上述两个 catalog 文件。
- 原工作区的同一 catalog 所列 677 个 prefab 全部存在。因此素材并非丢失，
  但只合并 Git 提交不会把这些本地素材交给其他维护者或 GitHub Actions。
- 开启 IncludeContent 后构建需要真实 prefab、材质、贴图、动画及依赖；
  索引不能代替资源。当前材质校验甚至可能先因缺 prefab 抛空引用异常。
- 建议：建立固定版本、校验哈希的外部素材包获取流程，保留原 `.meta`；
  或建立独立的、按平台发布的预构建资源包流程。后者需要调整当前
  `PreparedBundle` 对源资源 hash 的校验方式，不能只复制 bundle 就当接线完成。

### P1：CI 未接入闪卡版本矩阵及资源预构建

- `DynamicCardBuild.IncludeContent` 读取 `LEGACY_GWENT_DYNAMIC_CARDS=0/1`，
  其次读取 `ProjectSettings/DynamicCardsBuild.json`；均未设置时为 false。
- desktop、mobile、release 三个 workflow 都没有内容版本矩阵、资源获取、
  `PrepareForBuild` 调用或闪卡专用构建方法，也没有区分两种内容的产物名。
- Unity 标准 Build 窗口注册的 handler 会先构建资源，但程序化
  `BuildPipeline.BuildPlayer` 不会自动经过该窗口 handler。
  `OnPreprocessBuild` 只接受已准备且 hash 匹配的资源包，否则主动失败。
- 当前 CI 干净构建默认尝试无动画资源版；仅把开关改成 1 仍不足以构建闪卡版。
- 建议：统一一个 CI 构建入口，先准备目标平台资源，再构建 Player，最后
  恢复临时文件；矩阵与产物名显式区分 Windows/Android × 标准/闪卡。

### P1：Android 未启用 ARM64

- `ProjectSettings.asset` 的 `AndroidTargetArchitectures: 5` 是 ARMv7=1
  加历史 X86=4，不包含 ARM64=2；`scriptingBackend: {}` 未显式启用 IL2CPP。
  工程和 workflows 中未发现覆盖这两项的构建代码。
- 不能因此保证支持仅运行 64 位应用的安卓设备。5 是配置位掩码，
  不代表 Unity 2019 最终 APK 一定还会生成 x86 库；最终 ABI 需检查 APK。
- 建议：正式安卓路径显式设置 IL2CPP + ARM64（按支持范围保留 ARMv7），
  随后校验 SignalR/JSON 反射与 AOT 路径、原生 ABI 和真实 APK 启动。
- 参考：[Unity 2019.4 架构枚举](https://github.com/Unity-Technologies/UnityCsReference/blob/2019.4/Editor/Mono/PlayerSettingsAndroid.bindings.cs)、
  [Android 64 位兼容说明](https://developer.android.com/games/optimize/64-bit)。

### P2：无动画资源版仍包含完整闪卡界面

- 开关只控制 `StreamingAssets/DynamicCards` 中 `cards*.bundle` 和索引。
  Runtime 脚本及 `Resources/PremiumCrafting` 等小型 UI 资源依然随 Player 发布。
- `EditorInfo.Awake` 无条件创建 `PremiumCollectionPanel`；`PremiumFilter`
  默认 2，`CollectionVariants` 默认生成普通和闪卡两个展示项。
- `DynamicCardSettingRow.Install` 不判断包内资源是否存在，合成按钮也只检查
  服务端账户与费用。因此标准包仍可能显示不可播放的闪卡选项并允许合成。
- 缺资源时动画加载会退回普通卡图，不是这个分支必然崩溃；但它不满足
  “完全不使用闪卡的玩家界面基本不变”的更强要求。
- 建议：增加包内容能力标记，标准包默认只展示普通卡，隐藏不可用的动态
  画质入口，并明确决定是否保留合成入口。账户拥有状态仍由同一服务器维护。

### P2：Android 正式升级签名尚未固定

- `androidUseCustomKeystore: 0`，mobile/release workflow 未传入固定 keystore。
  文件中的 keystore 路径和 alias 不代表已启用该签名。
- 不同机器生成的调试证书可能不同，不能保证覆盖升级。两个内容版本若
  作为同一应用互相替换，应使用相同包名、稳定签名和一致的版本规则。
- 建议：通过受保护的 CI secrets 配置正式签名，再检查历史包证书与新包
  证书及覆盖安装。参见 [Android 应用签名](https://developer.android.com/studio/publish/app-signing)。

## Android 渲染与性能：风险，尚非已复现故障

- 本机 `Library/DynamicCardsBundles` 只有 `StandaloneWindows64`，25 个
  `.bundle` 共 1,452,231,666 字节（约 1.35 GiB）。该数字既不是 Android
  APK 大小，也不是运行时内存；Android 必须单独生成资源包。
- 自动纹理优化只写 Standalone DXT5 Crunch。抽查原工作区
  `Old/Thronebreaker/OriginalTextures` 的 276 个 PNG meta：没有 Android
  override，275 个有 Standalone override。不能将这个样本视为全素材统计，
  也不能据此断言 Android 一律未压缩；它仍会使用 Unity 的平台默认设置。
- 建议显式制定 Android 透明纹理格式、尺寸和 GPU 范围。ASTC/ETC2 的
  支持范围及不支持时解压的内存代价，见
  [Unity 平台纹理文档](https://docs.unity3d.com/2019.4/Documentation/Manual/class-TextureImporterOverride.html)。
- 754 个动态卡 shader 中 748 个声明 target 3.5，均有 PortableCard fallback，
  未发现 only_renderers/exclude_renderers 平台排除。target 3.5 对应 ES3 级别；
  fallback 只能提供另一渲染实现，不能证明材质、后处理与原效果一致。
  `DynamicCardPostEffect.Prepare` 也没有能力分级。
  参考 [Unity shader target 文档](https://docs.unity3d.com/2019.4/Documentation/Manual/SL-ShaderCompileTargets.html)。
- 高/中/低/关会改变 RenderTexture 尺寸及重绘频率，但不会减少源贴图
  分辨率或可见卡数量。低档不能单独保证低内存设备稳定。动画/粒子仍有
  CPU 成本。现有 Windows 基准不能当作安卓性能通过。

## 已正确接入的基础机制

- Bundle builder 按 BuildTarget 生成 `Library/DynamicCardsBundles/<target>`，
  没有把 Windows 资源路径硬编码为 Android Player 的加载路径。
- Android 的 jar URL 使用 UnityWebRequestAssetBundle 和 UnityWebRequest
  分别读资源及索引，符合
  [StreamingAssets 平台规则](https://docs.unity3d.com/2019.4/Documentation/Manual/StreamingAssets.html)。
- 无动画资源构建会暂移已有动态包，构建后恢复；动画资源缺失时保留静态图。
- AI Android manifest 后处理加入 INTERNET 与 cleartext HTTP 许可，
  默认服务器地址来自 Resources/ServerEndpoint.txt。
- 所有客户端 CI 在 Unity 构建前重新生成 netstandard2.0 Common DLL，
  新客户端 DTO 不依赖 net10.0 服务端程序集。
- Runtime 中本次检查到的 UnityEditor 引用有 UNITY_EDITOR 条件保护，
  `Assets/link.xml` 已保留 Assembly-CSharp、Common、SignalR 与 JSON 程序集。
  这仍不等于已验证切换 IL2CPP 后的所有调用。

## 建议的处理顺序

1. 接通可复现素材来源和四种产物的构建入口，这是目前的直接交付阻塞。
2. 完成无资源包的界面能力控制，明确标准包的体验。
3. 补 ARM64/IL2CPP 和稳定安卓签名，明确图形及纹理基线。
4. 后续即使暂时没有真机，也可编译四种产物并检查 APK ABI、签名、
   manifest、资源清单及大小；最后再做手机视觉、内存、温度和实际对局验收。

本次只记录检查结论；未修改运行代码、CI、资源开关或签名配置。
