# 动态卡模块

游戏内设置“动态卡”开启后，有对应素材的卡牌按页面显示顺序逐张替换；没有对应素材或关闭设置时保留普通卡。无需运行额外的性能测试场景。工具栏 Login 按钮可从登录入口开始运行。

## 内容与覆盖范围

2026-09-12 当前素材目录共 740 个场景（Thronebreaker 255、Legacy2017 324、Latest 161），670 个唯一卡图映射中，651 个属于现有的 671 个游戏卡图。其余 20 个保持静态，具体范围见 `work/DynamicCards/RuntimeStability-20260912/static-card-scope.csv`。

`Content/catalog.json` 保存唯一对应关系。素材集中在 `Content/Old/Thronebreaker`、`Content/Old/Legacy2017` 和 `Content/Latest`；`sourceVersion` 记录来源，`sourceId` 保留源动画根节点编号。旧版优先，缺失的卡图从新版备份补回。打包器验证三个来源目录、重复映射、蒙皮骨骼、纹理属性绑定和源文件明确不参与绘制的空材质辅助表面。

2026-09-09 当前 Windows 资源包为 25 个内容分包，加目录包和索引共 27 个文件，1,741,039,063 字节（约 1.62 GiB）。保留 DXT5 Crunch 质量 80、原纹理尺寸、alpha 与色彩空间；通过恢复源纹理、骨骼和动画契约修复画面。当前验证记录位于 `work/DynamicCards/MotionIntegrity`；下文带日期的旧范围和体积是历史记录。

本轮修复包括：遗漏卡图映射、图集及共享纹理误绑定、同名不同动画混淆、零长度常量片段、错误循环标记、遗漏 Avatar 骨骼，以及空材质辅助网格误绘制。大卡、收藏小卡和牌组缩略图均保护异步静态图回调，防止对象回收或换卡后写回旧图。

开场双方领袖的 `MyCards` 也接入动态卡，沿用开场 Animator 的翻牌、移动和原有语音。右键详情的独立整卡节点负责动态预览，说明文字保持在外；异步静态图回调只更新最新选中的卡牌。

历史记录（已被上方三来源内容替代）：2026-09-08 完成旧版分包并恢复编辑器异步缓存：580 个场景、20 个内容分包，连同目录包和索引共 22 个交付文件，1,342,622,927 字节（约 1.25 GiB）。此前迁移校验将源文件明确为空的网格或 Animator 节点误判为缺失，阻断构建，导致编辑器回退同步读取；本次保留损坏引用检查并完成重建。

独立 Unity 运行验证使用正式运行脚本与上述完整缓存，从 17 个分包抽取 20 张卡：整页 2.762 秒，加载中打开已就绪小卡的大卡 13.2 毫秒，P95 帧时间 17.4 毫秒，最大 56.0 毫秒（1 帧超过 50 毫秒）。小卡继续播放、后台加载及离页释放通过；这不是完整收藏界面的手工验收，也不保证消除全部冷启动长帧。结果见 `work/DynamicCards/OldSourcesStage/performance-result.txt`、`performance-metrics.txt` 和 `delivery-audit-final.json`；实际详情层级及领袖画像播放验证见 `entry-test-result.txt`。本轮未重建玩家程序。

## 动画与画面

2026-09-13 后续纠正：单独关闭 Tempest 的 `water_front` 会连该表面的流动水纹一起移除，独立浪花覆盖范围不足，导致底部黑洞。当前保持这块固定前景浪形关闭，同时将后方海面的 54 个下部顶点及相应纹理坐标向下延展；`TempestSeaCoverage` 在原图集的有效水域内连续取样，避免直接拉长水纹或采到图集空白。独立 `FoamSplash` 及其边缘渐隐继续播放，船只和其他网格部分保留。换机后依次运行 `python work/DynamicCards/repair_tempest_edges.py --apply`、`python work/DynamicCards/repair_tempest_water_coverage.py --apply`，再重建动态卡资源包。分层证据及验证记录见 `work/DynamicCards/TempestWaterCoverage-20260913`；此前 `TempestWaterOff-20260913` 的黑洞版本已被此修正取代。

2026-09-13：Tempest（70093 / 202203）的前景浪花使用 `TempestFoam`，按 `SheenQuad` 的菱形 UV 边界提前渐隐，避免流动纹理到达斜边时被突然截断。前景水面使用 `TempestWaterSurface`，固定表面透明遮罩并保护无效流动采样；19 个前景顶点的透明度形成柔和上沿，水面和浪花按 3100 / 3101 的顺序合成。这是针对原素材硬边的局部显示调整。换机或重新转换素材后，可运行仓库根目录 `python work/DynamicCards/repair_tempest_edges.py --apply` 备份并应用，再重建动态卡资源包。连续帧对照、正式资源与运行验证记录见 `work/DynamicCards/TempestEdge-20260913`。

2026-09-13：莫斯萨克（Ermion，62004 / 15210300）的三个蒙皮网格恢复 2017 原素材的 `updateWhenOffscreen=false`。此前统一开启该选项会重算包围盒，使透明面部与躯干在部分姿势下交换绘制顺序，表现为脸和胡子被衣服遮住。原始骨骼、动画曲线、材质和包围盒数值均保留。换机后运行仓库根目录 `python work/DynamicCards/repair_ermion_skin_bounds.py --apply` 可备份并恢复这三项设置，然后重建动态卡资源包。诊断与实际画面记录位于 `work/DynamicCards/ErmionTempest-20260912`。

2026-09-11：亨赛特与布罗瓦尔·霍格的背景火焰使用预乘透明混合。四张共享火焰序列图必须关闭 TextureImporter 的 `alphaIsTransparency`，保留 PNG 透明区域的原始 RGB；开启该选项会扩散颜色，使粒子出现细条和矩形色块。贴图名称 `fire_13x5` 的实际内容是 10×10，不能据文件名修改粒子分帧。原图、粒子尺寸/朝向和 DXT5 Crunch 质量 80 均保留。

这四份本地 `.png.meta` 不随 Git 交付。换机后可用仓库根目录 `python work/DynamicCards/repair_fire_texture_import.py` 只读检查，追加 `--apply` 会先备份再修正四个已核实 GUID 的导入设置；随后通过现有入口重建动态卡资源包。现场对比和实际资源包验证记录位于 `work/DynamicCards/FireRepair-20260911`。

原始压缩曲线经解码后重建控制器层、默认状态、速度和无条件转场，保留入场、切割和循环时序。蒙皮使用四骨骼权重，场景偏移放在动画根节点之外。材质恢复源渲染队列、纹理通道和常量寄存器绑定；粒子保留源组件和阶段参数。长特效曲线不再受旧采样帧数上限限制。

大卡支持有限角度拖动、卡框同步旋转、松手回正，上下范围维持源范围的一半。声音遵循游戏音效开关；文件保留与运行时事件还原是不同层次，尚未完整复现原游戏的全部音频事件系统。

## 统一构图

2026-09-10 起，完整卡面按三个原客户端共同的 `CardRTRenderer/Card/Appereance` 层级还原：外观节点 Y=2，相机保留源 Z 和 FOV，显示区域由原始静态牌面平面投影计算。大卡、收藏小卡及拖动回正不再叠加此前的经验垂直偏移。三个缺少 CameraValuesChanger 的源场景显式使用原渲染器默认参数。横向牌组条目恢复原始 `_slot` 静态图；底层窄幅动态裁切接口仍保留兼容。

已核对 663 个映射场景、669 个卡图的相机来源，抽查 14 张卡；构图参数检查不等于全部卡牌的视觉验收。旧版 1,995 版本的构图测试属于历史记录，不能证明当前构图正确。最新修复范围、验证边界及本地素材恢复方法见仓库根目录 `work/DynamicCards/REPAIR_NOTES-20260910.md`。

## 加载与释放

页面先显示普通卡，等待布局稳定后按从上到下、从左到右的顺序串行加载。滚动和交互期间延后普通队列，大卡预览优先。大卡显示期间，可见小卡仍继续动画、粒子和每秒 24 次的错峰重绘，后续小卡继续按帧耗时调节的间隔加载。离屏卡释放实例，返回时重新排队；预览持有自己的资源租约，不再阻止无关分包回收。每个内容包最多包含 32 个卡牌场景，显示中的卡牌及加载中的请求持有引用；无人使用的包会释放，重新显示时再异步加载。

编辑器优先使用 `Library/DynamicCardsBundles/StandaloneWindows64` 中的异步包：`cards.bundle` 保存目录，`cards.index.json` 指向 `cards-*.bundle`。只有完整生成后才写入 `cards.bundle.editor-ready`。内容或着色器变动会使标记失效，包括资源包生成后的空白字符整理。没有有效缓存时默认保留静态卡，并在 Console 提示重建，避免自动进入会阻塞主线程的 AssetDatabase 加载。通过 `Tools > Dynamic Cards > Build Options > 仅构建动态卡资源包` 重建缓存后重新进入 Play Mode；输入未改变的分包会复用，失败后可继续构建。素材调试时可在同一窗口主动开启“开发用：允许同步读取原资源”，该选项按本机项目保存，默认关闭。异步加载不能消除 Unity 原生实例化和 GPU 上传的所有主线程开销。

## 可选构建

`Tools > Dynamic Cards > Build Options` 控制是否随游戏打包素材，默认不包含。开启时按目标平台构建 LZ4 分包，将目录和全部分包临时放入 `StreamingAssets/DynamicCards`，玩家构建结束恢复原文件。关闭时不随玩家程序带入这些动态包。运行时设置和构建开关独立。

CI 可设置 `LEGACY_GWENT_DYNAMIC_CARDS=1` 或 `0`。程序调用 BuildPipeline.BuildPlayer 前调用 `DynamicCardBuild.PrepareForBuild()`，并在 finally 调用 `RestoreStage()`。不同平台的资源包不能互换。

## 验证边界

以下仅新版整理、1,995 版本及 9.03 GiB 数据均为历史验证，不能替代当前补缺后的验收结果。新版映射及缺失卡图记录位于 `work/DynamicCards/LatestOnly`，最新压缩包结果位于 `work/DynamicCards/Optimization`。此次依赖扫描包含动画曲线，发现四份已存在的 Source 动画有同一个缺失脚本 GUID（`3e080548dacc61344a5969d8317f5acc`）；它不是删除旧目录造成的，尚未在此次资源整理中修复。场景去重不等于所有视觉效果已验收，也不等于跨分包共享依赖已经消除重复。

新版整理验证：正式 Content 恰有 1,279 个 Card.prefab，卡图无重复绑定；19,643 个 prefab/material/controller 引用检查无缺失 GUID 和重复 GUID。Windows 包内场景全为 Latest，总计 43 个发布文件（42 个 bundle 加索引），已替换本机编辑器缓存。隔离 PlayMode 使用新包验证杰洛特、伊格尼、金龙的动态首帧及随后画面变化，并验证旧版独有卡回退静态；这不是全量卡牌视觉验收。证据分别为 `content-result.json`、`client_guid_audit.json`（上一级目录）、`build-result.json`、`runtime-result.json` 和 `delivery-result.json`；完整卡图变更清单是 `card-mapping.csv`。

全量结构检查：1995 个场景，20302 个粒子系统，3493 个 Animator，问题数 0（`premium_structure_audit.json`）。另有 134 个槽位经原始场景确认本来就是空材质，单独计数；未启用的粒子拖尾槽位不视为丢失材质。741 个迁移着色器通过支持性和编译错误检查（`premium_shader_audit.log`）。

12 个重点版本做了连续画面检查。`geralt_timeline_final.log` 验证真实运行组件的完整水鬼、切割、持久循环和重播；`focused_partition_player.log` 在独立运行版验证 60 个请求的顺序、预览优先、暂停后台动画、离屏释放、跨包隔离、加载途中取消及资源包重载。该小规模测试按每包 4 个场景构建，以覆盖跨包情况；正式默认最多 32 个。`focused_partition_stage.log` 验证构建开关关闭/开启、全部分包随程序发布及原文件恢复。

整理前的运行包曾有 66 个发布文件、9.70 GB（9.03 GiB）。`premium_full_bundle.log` 验证当时全部 1,995 场景；`premium_full_queue.log` 验证当时完整包的队列、预览暂停、跨包释放、加载途中取消和重载；`geralt_full_partition_timeline.log` 验证当时完整包中的杰洛特入场、切割、循环和重播。纹理优化前，新版唯一场景包的重建结果为 1,279 场景、7.01 GB（约 6.53 GiB），记录于 `work/DynamicCards/LatestOnly/build-result.json`。

这些证据不等于逐帧验收全部 1,995 个版本，也不等于完整游戏或手机真机的性能保证。部分源控制器条件转场、AvatarMask、专有脚本和音频事件仍需按卡牌继续核对。兰伯特的部分黑色前景在直接加载原始 Unity 2022 场景时同样存在（`OriginalLambert`），该对照没有包含原游戏完整收藏 UI。旧版 12230611 的 mesh_middle 第 114 根骨骼在源文件中本就缺失，按原始绑定矩阵恢复了保底姿势，不能声称恢复了该骨骼的独立动画（`source_bindpose_fallback.txt`）。Latest 15760101 则从原始 Avatar 默认姿势补回缺失骨骼并重新绑定动画。

转换与审计工具在 `work/DynamicCards`，不属于发布资源。源游戏安装目录未修改。着色器转译使用 HLSLDecompiler 及原始 Unity 参数表；重新转换应保留源导出及还原记录，避免覆盖已验证的映射与材质状态。

## 2026-09-07：静态重影与看似停播修复

动态卡使用独立 UI 合成材质：场景内的透明图层已经合成到相机 RGB，最终卡面不再重复使用相机 Alpha 混入底下的静态占位图。保留 UI 颜色、透明度、遮罩和关闭设置后的静态回退；大卡和小卡共用。Shader 放在模块自己的 Resources 中，无需重建动态素材包。

旧版场景的背面停放朝向在没有原版显式启动变换时恢复正面，修复亨赛特 12110300 的模型朝后问题；保留三份原版显式指定的 180 度变换。全量检查了 1,995 个场景根节点。哈罗德 15110300 的源动画在实际素材包中能够播放，本次验证了其大卡、小卡骨骼和渲染画面随时间变化。

回归：Unity 2019.4.1f1 隔离项目加载正式素材包，对艾瑞汀、暗影长者、亨赛特、哈罗德分别验证大卡/小卡，共 8 项通过；静态占位图换色后动态区域差异像素均为 0，动画前后均有骨骼及画面变化，关闭动态设置恢复静态图。详见 work/DynamicCards/PremiumIssues/verification.json 和 premium_regression.log。未逐张运行全部 1,995 个版本。

## 2026-09-07：选中大卡加载优先级

大卡不再等待小卡的 0.25 秒防抖、2 帧布局等待和 0.15 秒队列间隔，使用正常异步加载优先级；已有小卡加载任务会在完成读取后让出显示任务。大卡省去额外的分帧显示等待。小卡列表继续串行、分段加载。

已加载的模型、声音按素材分包缓存，选中同一张小卡时复用资源；分包没有使用者后同时释放缓存，不常驻保留全部卡牌。仍使用异步素材读取，不强制同步磁盘加载。

正式素材包实测：已加载小卡打开大卡 11.8ms，重访 18.0ms；该次测试另一张首次预览 46.1ms、快速切换 85.2ms。数字是本机测试结果，不代表所有首次磁盘读取。60 个请求的排序、分段间隔、预览优先、离屏卸载、分包隔离、取消和重载检查通过。记录位于 work/DynamicCards/PreviewLoading/verification.json。

### 悬停预览入口补齐

收藏页/牌组的 EditorInfo.SelectSwitchUICard 经 ArtCard.SetCard 显示右侧大卡；它与 righclickLogic 的右键查看是不同入口。ArtCard 现显式使用 largePreview=true，接入即时动态首帧加载；保持原先悬停不自动播放音效的行为。此前仅加速 largePreview 队列并未覆盖此调用点。牌组/墓地共用的 ArtCard 同样生效，列表 CardShowInfo 仍走分段加载。

验证计时终点是动态 RawImage 首次启用，不是静态大卡的出现。正式素材包测试暖资源 12.3ms、重访 16.3ms，并检查快速切换后最终卡 ID、关闭设置回退及无悬停音效。详见 work/DynamicCards/HoverPreview/verification.json。

## 2026-09-07：动态就绪后显示、原版侧翻与更积极的小卡加载

有动态资源的预览在等待首帧时隐藏整张卡（含框）；完成动态渲染后才整体显示。等待时保留布局和加载资格，仍尊重外层隐藏状态。禁用动态卡或没有对应资源时显示原静态卡，不永久隐藏。ArtCard 悬停入口传入整个展示根节点；右键查看使用已有整卡根节点。

原版 E:/Hbackup/FileRecv/Gwent/Gwent 的 UISidePreviewCard 与 deckbuilder_base 中 SidePreviewCardAnimationSettings 提供了右侧入场的 55 度转角、1 秒回弹曲线；主要转动集中在前段。当前保留该角度及曲线，将原世界坐标移动适配为 UI 相对位移/缩放。框和内容同时旋转，快速换牌丢弃旧请求；拖动会接管未完成的入场动画。原版客户端未进入收藏界面，本次依据本地代码与序列化配置适配，未作原客户端动态画面的逐帧对照。

小卡初始等待由 250ms 缩短为 80ms、布局等待降为 1 帧；卡间间隔由固定 150ms 改为根据平滑帧耗时在 25–100ms 调整。保持逐张加载、优先预览、离屏释放。

Unity 2019.4.1f1 使用正式素材包通过：首帧前隐藏、整卡侧翻回正、快速切换仅最后一张可见、关闭/缺资源回退、外层隐藏、拖动接管，以及 60 请求排序/分段/卸载/取消/重载。详见 work/DynamicCards/PresentationDelivery/verification.json、presentation_final.log 与 adaptive_thumbnail_queue.log。


### 2026-09-07 Preview pivot and stationary description

ArtCard now creates a runtime DynamicCardVisualPivot centred on CardBorder, moves card graphics beneath it, and keeps CardContent outside it. Serialized references and the existing layout root remain intact. DynamicCardPresentation still gates the whole preview until the first dynamic render, but rotates only the visual pivot. Removed the previous arbitrary 6% horizontal translation and shrink. Entry uses the extracted 55-degree, one-second curve. Idle oscillation is a video-based adaptation bounded to the original PerspectiveParams defaults (pitch 1.5 degrees, yaw 7 degrees), not an extracted original idle curve. PreviewEdgeGlow is a lightweight procedural cyan UI outline, not the original particle asset. Static fallback hides the glow and restores rotation.

Evidence: work/DynamicCards/PreviewPivot/test.log has PRESENTATION_PASS from isolated PlayMode using installed card bundles: gate, frame/art rotation, stationary description, stable centre and scale, idle/glow, rapid switching, disabled/missing fallbacks and ancestor visibility. Main client's collection page was not visually exercised in this verification. Original hierarchy read from deckbuilder_base: UISidePreviewCard root -> CardTransform -> CardContainerTransform; CardRotationController modifies CardContainerTransform. Original video contact sheet is in PreviewPivot/contact.jpg.

## 历史记录：2026-09-07 Desktop source checkpoint and external content

The source includes the integration, runtime/editor scripts, shaders and latest-only catalog. Extracted models, textures, animations, audio and particle assets in `Assets/DynamicCards/Content/Latest` are local external content (approximately 43.76 GiB after texture consolidation), excluded from Git. Their existing files and Unity .meta GUIDs must be backed up together. The catalog alone cannot recreate them. The earlier commit a852fc646 documented a three-source, approximately 78 GiB content snapshot.

To reproduce this workstation's dynamic cards on another machine, copy the complete current Content directory with its .meta files into the same project path. For editor playback, also copy the matching `Library/DynamicCardsBundles/StandaloneWindows64` (including index and editor-ready marker), or rebuild it through the module's Build Options menu. Do not reuse the old 1,995-scene bundles with the new catalog. Windows player builds can optionally include these bundles. Without the external content/bundles, this source checkpoint is not a complete dynamic-art distribution; static-card fallback remains available.

The unfinished local Android build workspace has been removed and its runtime external-content loader reverted. Desktop evidence above is limited to the stated audits and isolated tests; it does not establish full-game or all-card visual acceptance.

## 2026-09-07：Windows 包体积与小卡加载修复

保留全部 1,279 个新版场景及唯一映射，发布包由 7,011,978,254 字节（6.53 GiB）降到 3,285,218,316 字节（3.06 GiB），减少 53.1%。43 个发布文件均经过读取和场景名单验证。

只合并 PNG 内容及导入设置完全一致的重复贴图：10,024 份减至 5,075 份，材质引用和转换记录同步更新。Windows 常规贴图改用 DXT5 Crunch、质量 80，保留原分辨率上限、mipmap、色彩空间及 alpha 设置，极小常量贴图保持原格式。剩余源 PNG 像素未改写；运行包压缩是有损的。8 张大贴图抽样的最大平均 RGBA 误差为 0.009865，尺寸不变；这不是全量卡牌的视觉验收。动画曲线、模型和粒子参数未为压体积而删减。

编辑器缓存现在正确处理 `.meta` 自身的刷新及新包已不包含的旧文件删除；真实内容或导入设置改动仍使缓存失效。修复了错误失效后退回同步 AssetDatabase 读取的退化。无可用包时会输出明确警告。逐张加载、显示顺序、预览优先及离屏释放策略保持现有行为。

分包复用同时改为依赖文件及 `.meta` 的 SHA256 指纹；按文件大小和修改时间缓存已计算指纹，依赖变更时强制重建该分包，未变更则复用。回归实际验证了未变更包保持不动，以及依赖贴图的导入设置改动会重建并进入最终包。

同一台机器、Unity 2019.4.1f1 隔离 PlayMode、同样 20 张中立金卡的页面：

| 加载路径 | 全部动态首帧就绪 | P95 帧耗时 | 最大帧耗时 |
| --- | ---: | ---: | ---: |
| 缓存失效，直接读取编辑器素材 | 22.52 秒 | 346.5 ms | 9914.1 ms |
| 原有效异步包 | 2.87 秒 | 17.5 ms | 217.7 ms |
| 本次压缩后的有效异步包 | 2.80 秒 | 17.5 ms | 205.7 ms |

每次使用新 Unity 进程，未控制操作系统磁盘缓存；结果不代表所有设备或正式收藏 UI 的保证。新包另外通过杰洛特、伊格尼、金龙的动态画面变化及缺失资源静态回退检查。缓存误失效、依赖变更重建、19,643 个场景/材质/控制器引用检查均通过。详细本地证据在 `work/DynamicCards/Optimization`：`build-result.json`、`page-optimized.json`、`cache-test.json`、`bundle-cache-test.json`、`runtime-result.json`、`final-guid-audit.json`、`delivery-result.json`。

当前源 Content 约 43.76 GiB；这是编辑用模型、动画和源贴图，不能与压缩后的发布包混为一个口径。源码提交仍不包含被忽略的 Content 素材及 Library 资源包；完整素材及其 `.meta` 需要单独备份。本机正式缓存已替换为本次验证的版本。首次编辑器刷新导入设置可能产生一次性导入开销。

## 2026-09-08：静态回退与预览期间小卡暂停修复

取消“大卡可见就停用所有小卡模型、重绘和加载”的全局限制；保留大卡加载优先、小卡串行异步加载、24 Hz 错峰重绘及离屏实例释放。

依据新版客户端 `Templates.xml` 的 Template Id → ArtId 明确对应关系，补回 11 个遗漏映射：夏妮、艾达·艾敏、海玫家族保卫者、呢喃婆：贡品、文登达尔精锐、呢喃山丘、伊欧菲斯：冥想、贝克尔的岩崩术、莫丽恩：森林之女、叶奈法：死灵法师、伊勒瑞斯：临终之日。这些映射指向现有新版场景；部分新版名称和旧版名称不同。现有游戏卡图覆盖由 618/671 增至 629/671，未添加重复场景。

对压缩包全部 1,279 个场景实例化并采样 Animator：1,204 个有变换变化，75 个未检测到变换变化但均包含粒子系统；与原始导出的动画记录比较，没有发现原始记录有曲线而包内完全没有有效动画片段的场景。这是结构与动画采样检查，不是全部卡牌的逐帧视觉验收。

隔离 PlayMode 使用交付的压缩包检查了上述 11 张卡、雷索：弑王者、Kingslayer 和杰洛特共 14 张的入场采样与循环画面变化。三张小卡在所有大卡预览期间持续推进且画面变化；预览期间新增小卡加载和离屏实例释放通过。另一个 20 张小卡加大卡预览的页面测试全部加载耗时 2.59 秒，P95 帧耗时 17.4 ms，最大加载帧耗时 213.8 ms；不代表所有页面或设备没有加载峰值。

本地证据在 `work/DynamicCards/AnimationRegression`：`restored-bindings.json`、`bundle-audit.json`、`audit-summary.json`、`runtime-result.json`、`page-result.json`、`delivery.json`，并保留了测试脚本。正式缓存 43 个发布文件与验证源逐一校验，只有 `cards.bundle` 目录包发生变化，场景分包完全未变；总计 3,285,218,367 字节（约 3.06 GiB）。当前已开始的 PlayMode 需退出后重新进入，才能重新读取修正的映射。

## 2026-09-08：遗漏映射与旧版补缺复查

补回 14 个新版精确图像匹配和 7 个旧版独有映射。此前部分静态图含有不透明白边，匹配裁切仅处理黑边，导致已有新版素材未被对应；另有旧版映射在仅保留新版整理时移除。本次从原始内容和转换备份恢复，不以相邻编号替代不同卡图。篡位者 20158000 与尼弗迦德大门 20055600 使用隔离旧版补缺；吊死鬼之毒 20154000 对应新版 15860101（Cadaverine）。

隔离 Unity 2019 编辑器从压缩分包检查全部 671 个游戏卡图编号：650 个已映射卡图的循环采样渲染画面有变化，21 个未匹配。再对本次 21 个补回映射及通敌、兰伯特进行 23 张自然播放回归，验证入场采样和循环画面变化、预览期间三张小卡继续播放、后续小卡继续加载及离屏释放。使用源 Animator 的实际角色入场时长复查长特效控制器，不把测试的时间跳转或粒子 Simulate 暂停误判为运行时停播。结果分别为 `AnimationRegression/all-mapped-motion.json` 与 `fallback-runtime-result.json`；画面随时间变化不等于每个角色和特效均已逐帧视觉验收。

补缺包约 22.9 MiB，原有新版场景分包保持不变。正式缓存发布及哈希检查见 `AnimationRegression/fallback-delivery.json`。自然播放测试 P95 帧时间约 17.53 ms，但首次加载最大帧仍达 555 ms，不能据此宣称加载卡顿完全消除。
补缺源资源的共享依赖另外恢复 199 个文件及元数据，集中在 Content/Fallback/Recovered，确保正式项目重建时不依赖转换工程。补缺目录 188 个 prefab/material/controller 文件最终引用检查无缺失，正式项目 GUID 索引无重复；证据为 fallback-dependency-recovery.json、fallback-final-guid-audit.json 和 fallback-duplicate-audit.json。上述依赖原本已在测试分包内，不增加发布包体积。

## 2026-09-08：旧版金龙背景与伊勒瑞斯溶解修复

伊勒瑞斯 `13210201` 的 Mesh1 控制器曾被转换工具自动添加“入场 → 空循环状态”跳转。原始控制器没有这条跳转；空状态的 Write Defaults 把 `_Progress` 恢复为材质默认值 0.324，造成一直半溶解。移除该跳转，入场片段结束后保留 0，主体正常进入循环。工具侧 `work/DynamicCards/SourceAnimationImporter.cs` 同步禁止向空 motion 的循环状态合成跳转。全部旧版控制器记录中，同类空循环误跳转仅发现这一例。

金龙 `11210701` 原版通过 `VFXTileMotion` / `VFXTile` 动态实例化并滚动燃烧森林地块，原静态导出遗漏外部 TileA / TileC 模板。现已从旧客户端提取模板、网格、材质、贴图及粒子，集中在该牌的 `Tiles` 子目录；`DynamicCardTileMotion` 负责循环移动、逐实例弯曲中心和噪声 UV。使用原始速度、间距、模板数量，并向前景延伸一个地块覆盖当前肖像裁切。隐藏时随卡牌暂停，重新显示不会重复生成地块池。

正式缓存仅重建 `cards-legacy2017-000.bundle`、`cards-legacy2017-002.bundle`。22 个发布文件共 1,344,321,791 字节；校验清单及 editor-ready 已更新。独立 Unity 2019 PlayMode 直接读取这些交付分包，验证金龙滚动背景以及伊勒瑞斯入场和循环阶段的溶解参数；测试未注入替代控制器或背景。证据在 `work/DynamicCards/VisualRepair/Delivered`、`delivery-audit.json`。

另外对现有全部 580 个场景逐一自然播放并采样渲染画面：580 个成功加载，579 个发生画面变化。唯一静止的 `15430100` CastleGate 场景在原始导出中即无动画或粒子，且未映射当前游戏卡图。结果见 `all-motion.tsv`、`all-motion-summary.json`。短时间画面变化筛查只能识别整张静止，不能替代所有局部动画和特效的逐帧视觉验收。

挠挠爵士（当前图号 203081）仅在新版备份中发现动态场景 38100101，当前旧版目录没有对应场景；本次遵守既有“只使用旧版资源”要求，未自动恢复新版内容，仍为静态回退。

## 2026-09-08：全量播放时序修复

卡希尔 `16210401` 的刀光 LightAnimation 原始长度为 0.55 秒且不循环。转换工具此前将单状态控制器一律设为循环，并依据名称中的 Intro/Loop 猜测其他片段，造成反复刀光及部分本应循环的效果停止。现按原始控制器的循环标记恢复 78 个场景、115 个片段：97 个恢复单次播放，18 个恢复循环。源控制器没有跳转时也不再合成跳转。

以片段资源编号区分同名动画，修复 11590100、15660100、13230201、13221601 四个场景的片段覆盖；同一编号的重复引用先去重，不当作不同动画。补回 11780100、15110100 中遗漏的控制器状态。转换器遇到缺失状态资料或尚未消歧的重名片段会报错，而非继续猜测。

对全部 601 条粒子事件按源状态进入时间、速度、循环周期和过渡偏移重新计算，包括自跳转及进入状态时已越过事件位置的情况。601 条均有确定对应，生成器在隔离目录重放后与交付目录完全一致。源动画本来不循环、以及状态退出前根本没有到达的事件，不会被误当作持续循环。

最终独立 Unity 2019 进程从实际交付分包核对全部 580 个场景、1,316 个有效动画片段，循环标记和片段长度无差异；卡希尔实播验证刀光结束后的 12 次采样保持末帧，主体动画继续运行。该检查针对播放时序，不替代所有卡面局部视觉细节的逐帧验收。

正式缓存 22 个发布文件合计 1,344,434,418 字节，editor-ready 和内容校验清单已更新；额外复核本次涉及的 220 个资源文件与清单一致。未创建新的游戏安装包。证据与转换工具备份位于 `work/DynamicCards/TimingRepair`，主要结果为 `summary.json`、`contract-result.txt`、`cahir-result.txt`、`schedule-audit.json`，包含完整修改清单及还原用备份。

## 素材交付与重建保护（2026-09-09）

当前 Content 素材与 `.meta` 是配套的本地外部内容，Git 中的目录及源码不能独立重建素材。迁移时复制完整 Content 及 `.meta`，再复制匹配的 Windows 资源包与 `editor-ready` 标记，或在 Unity 中重新构建。不要混用本文历史记录中的旧目录包。转换、审计脚本与测试工程属于本地工具，不属于游戏发布资源。

`conversion.json` 的纹理赋值可显式指定 `texture`，用于源材质自己的纹理及跨卡共享纹理；未指定时使用该卡图集。构建前同时检查空引用和非空但错误的纹理引用。源文件原本没有材质的辅助表面通过 `nonRenderingPaths` 精确匹配，只关闭绘制，保留粒子、变换与动画；同名层级的有效渲染器不受影响。加权蒙皮缺失骨骼会阻止构建。源动画转换按片段身份而非显示名称区分资源，未解析的控制器契约会报错。

本轮边缘复核另发现狄杰斯特拉 12210501 的背景不覆盖默认取景上沿。目录中的 `verticalFramingCorrection` 修正整卡垂直取景，保持原视野角、模型大小和纹理；窄缩略图通过独立的 `thumbnailFramingCorrection` 将取景从头顶移回面部。未配置修正的场景使用默认值 0。四个拖动方向和长时间播放单独复核，避免仅检查静止正面。

## 2026-09-09：长列表与大小卡取景收尾

长列表在大卡预览可见时也回收零引用分包，保留正在显示的卡所持有的引用。爱丽丝的同伴（卡图 `20008300`）使用新版完整场景 `13860101`。整卡取消统一向上偏移，大卡与小卡使用相同的源相机取景，并按卡框宽度的 2% 对齐顶部边距；窄牌组缩略条保留独立取景。背景不足的 112 个来源场景使用各自的整卡修正，覆盖 114 个卡图。

`dragFramingCorrection` 指定拖动边界所需的最小垂直取景修正，仅卡图 `20011300` 和 `202283` 配置了此值。运行时从静止取景平滑过渡，松手回到原来的 `verticalFramingCorrection`；不改变默认相机位置、视野角、模型或动画。其余卡使用原有拖动余量。

本地复核与截图位于 `work/DynamicCards/MotionIntegrity/ResumeFraming-20260909`。测试必须同步实际资源包，并校验加载的目录参数和交付文件哈希；只复制运行时代码而沿用旧测试包，不能证明当前版本的取景结果。

最终实际包回归完成：118 张卡、354 个显示用例、772 张静止与拖动截图，顶部覆盖阈值异常为 0；自然播放、重新启用、松手回位与目录参数检查通过。四张重点卡在真实收藏 UI 中复核了大小卡相机、投影及顶部边距一致，爱丽丝同伴大小卡均有画面变化。全部 27 个交付文件哈希匹配，清理临时编辑器探针后的编译通过。详细范围、阈值和上一轮长列表记录见上述目录的 `RESULTS.md`。

## 2026-09-11：终末之战素材回退与狐妖云层绑定

终末之战卡图 `11310100` 恢复使用 `Latest/10400101`，包括原先的骨骼动画、特效与相机设置。9 月 8 日批量旧版切换曾将它替换为 `Legacy2017/11310101`；后者没有相同的人物骨骼动画，并出现火焰方块。旧场景保留为未映射条目以维持现有旧版分包分组。资源闭包、备份和交付验证位于 `work/DynamicCards/RaghRepair-20260911`。

狐妖：真身 `20005600` 的黑方块来自云粒子错误使用整张卡的图集。同名的两个云材质在源文件中是不同对象：粒子使用 `clouds_4x4`，流动背景使用卡面图集。`conversion.json` 的纹理绑定新增可选 `materialAsset`，导入及构建校验按具体资产区分同名材质；未指定时保留原来的名称匹配。修正仅恢复云粒子纹理，正常碎屑和粒子参数保持不变。源材质证据和隔离画面位于 `work/DynamicCards/AguaraRepair-20260911`。

本轮正式缓存重建及校验通过；独立 Unity 2019 进程直接读取主工程分包，完成两张卡大小预览的 20 次采样，覆盖超过 18 秒的持续播放和重新启用。86 个修正相关源文件及元数据与缓存清单匹配，27 个发布文件合计 1,743,396,631 字节，未涉及的分包哈希不变。最终结果见 `RaghRepair-20260911/Delivered`、`delivery-audit.json`；未创建新游戏安装包。

## 2026-09-11：特莉丝与金龙火焰修复

特莉丝·梅莉葛德 `11210600` 保留 `Legacy2017/11210601`。`11210601_20908_bigFire_7x5.png` 将动画帧分存在 RGBA 通道；透明颜色扩张会改写这些通道，导致火焰长条、矩形边缘。仅将该贴图的 `alphaIsTransparency` 恢复为 0，保留 PNG、压缩、材质、粒子参数。已将此贴图身份加入 `work/DynamicCards/repair_fire_texture_import.py` 的受保护清单。

维伦特雷坦梅斯（金龙）`11210700` 恢复此前使用的完整 `Latest/10130101` 场景，包括骨骼动画、森林、地面特效与源相机。旧版地面出现大片白色过亮区域；原先新版包的对照播放保留了火焰和地面细节。旧 `11210701` 场景保留为未映射条目，避免改变旧版分包分组。恢复闭包为 38 个缺失资产，现存依赖复用；备份、参考画面与验证位于 `work/DynamicCards/TrissVillenFire-20260911`。

正式缓存构建通过。独立 Unity 2019 进程直接读取主工程新包，完成两卡大小预览 20 次采样、超过 18 秒播放及重新启用验证；骨骼动画正常。特莉丝新包火焰贴图的 1,048,576 个 GPU 像素与正确导入参考完全一致。80 个相关源文件及元数据与缓存清单匹配，28 个交付文件合计 1,745,510,084 字节；未涉及分包的哈希保持不变。详见上述目录 `RESULTS.md` 与 `delivery-audit.json`。

## 2026-09-12：运行时动画依赖稳定性

修复资源刷新后同包中尚未使用的卡牌可能出现控制器片段归零、主体停止而粒子继续的问题。分包显式包含控制器资源，首次打开时异步加载并保留控制器和动画片段，随分包租约一起释放；模型、纹理和声音继续按需加载。索引升级为 v2，旧包需要通过现有构建入口重建。

全量运行覆盖 651 个动态卡图；594 个检测到骨骼运动，其余 57 个检测到画面变化。25 个内容分包加载途中刷新通过；筛选、滚动、卡组编辑、匹配房间、本地实际战场、调度、牌库、己方墓地、详情和关联卡入口通过。相同运行脚本的独立 Windows 程序完成 29 个卡图、57 项检查。完整 Windows 游戏已重建，28 个包文件与验收缓存逐个核对 SHA-256。证据、静态范围及验证边界见 `work/DynamicCards/RuntimeStability-20260912/运行稳定性验收.md`。
