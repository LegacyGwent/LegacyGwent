# 闪卡画质档位

## 产品约定

设置菜单的「闪卡画质」提供关闭、低、中、高四档，独立于整体画质设置。
所有可见且允许展示的闪卡继续播放：没有播放名额、同屏数量上限或静态替代策略。
不改变 Screen 分辨率、全局 QualitySettings、游戏目标帧率、账号所有权或网络外观选择。

| 参数 | 高 | 中 | 低 | 关闭 |
|---|---:|---:|---:|---|
| 小卡 RenderTexture | 384×384 | 256×256 | 192×192 | 无 |
| 小卡重绘目标 | 24 次/秒 | 15 次/秒 | 10 次/秒 | 不重绘 |
| 大图 RenderTexture | 1024×1024 | 768×768 | 512×512 | 无 |
| 大图重绘目标 | 每个游戏帧 | 30 次/秒 | 30 次/秒 | 不重绘 |
| 原后处理工作宽高 | 卡片 RT 原尺寸 | 卡片 RT 的 1/2 | 卡片 RT 的 1/2 | 不执行 |
| 模型/粒子/动画/音频素材 | 当前素材 | 相同 | 相同 | 不加载动态内容 |

高档保留原有渲染尺寸、24 Hz 小卡调度、逐帧大图、原始两次后处理 pass。
中低档保留全部后处理类型及两次 pass，在降低分辨率的工作纹理上执行，再放大合成。
没有一刀切关闭主体粒子、遮罩、溶解或改变骨骼权重。

重绘频率是上限目标，受实际游戏帧率限制；它不改变动画时间推进速度。
中低档暂时保留原有 Animator、粒子和脚本的模拟频率，主要节省相机提交、填充和后处理成本。
骨骼、粒子模拟和源素材内存仍有开销；不能把像素工作量下降当作整机 FPS 同比提升。

### 中、低档的取舍依据

中档保留 768 大图和 30 Hz 预览，主要降低列表里的小尺寸重绘成本；小卡输出像素/秒约为高档的 27.8%。
低档把小卡压到 192 和 10 Hz，作为明显更省资源的档位；小卡输出像素/秒约为高档的 10.4%。
低档仍保留 30 Hz 大图，避免放大查看和拖拽时也只得到 10 Hz 的视觉反馈。
这里的像素数量只统计卡片最终 RT，不包含几何复杂度、透明叠加及多次后处理，属于工作量指标。

代价是中档小卡动画稍有阶梯感，低档更明显，细节也会更软。它们是资源节省优先的初始预设，
不是已经在所有手机上确定的最佳参数；参数集中在一个 profile 文件，后续可按真机测试调整。
如果手机主要受骨骼/粒子模拟或源贴图内存限制，本轮收益会小于填充率受限的设备。

## 实现边界

- `DynamicCardQuality.cs`：稳定持久化枚举与集中、不可变的四档参数表。
- `DynamicCardSettings.cs`：保存 `DynamicCards.Quality`，旧 `DynamicCards.Enabled=1` 回退为高，旧关闭或新安装保持关闭；保留 bool 接口供原有逻辑使用。
- `DynamicCardSettingRow.cs`：接入现有选择器，四语言标签；语言刷新不会覆盖档位或触发整体画质回调。
- `DynamicCardView.cs`：在下一次调度重绘时应用尺寸，切换中/低/高不重建模型、不重新加载包、不重播动画或音频；释放旧 RT。
- `DynamicCardPostProcessRenderer.cs`：高档原路径，中低档缩小后处理工作纹理，临时 RT 用后释放。
- `DynamicCardRenderMetrics.cs`：显式开启的 CPU 相机提交计时和像素/重绘计数，正常游戏不启动计时器。

现有屏外暂停、延迟释放、异步创建、加载取消与资源包引用计数继续使用。
尚未实现 CPU 动画降频、逐卡装饰粒子精简、源贴图 mip 驻留控制或额外低清素材包。
在真机证据显示这些部分成为瓶颈前，不增加第二套素材库。

## 可重复验证

Unity 菜单：

- `Tools > Dynamic Cards > Quality > Run repeatable benchmark in Play Mode`
- `Tools > Dynamic Cards > Quality > Build Windows benchmark player`

工具生成独立临时场景，使用生产 `DynamicCardView.Bind` 和异步 AssetBundle 路径。
负载固定为 24 张小卡和 1 张带后处理的大图，覆盖三个资源来源和复杂卡牌；全部可见。
预热后分别按高→中→低及低→中→高测量，每个档位每轮 12 秒。
工具临时关闭 VSync/帧率上限以暴露吞吐差异，结束恢复设置；不改变测试期间的屏幕分辨率。
比较用同一机器、分辨率、卡牌列表和构建，不能跨机器直接比较 FPS。
此场景覆盖闪卡生产加载/渲染路径，不覆盖完整战斗、联网或整个收藏界面的 UI 开销。

输出位于 `work/DynamicCards/QualityBenchmark/{Editor,Standalone}`：

- `result.json`：平均、P95/P99 帧耗时，帧率，长帧，逐卡重绘次数，RT 内存与 Unity 分配量。
- `same-frame-*.png`：同一模型/动画时间点的各档大图输出。
- `progress.txt`：进度及 PASS/FAIL。

`renderSubmissionMsPerFrame` 是 CPU 调用 Camera.Render 的耗时，包含可能的等待，不是 GPU 执行耗时。
`meanGpuMs=-1` 表示当前平台/图形 API 没有提供有效 GPU 计时，不用 CPU 数据冒充 GPU 数据。
`renderTargetNativeBytes` 是 Unity 对持久卡片 RT 报告的本机内存，不是整进程显存；不包含源贴图、动画、临时后处理池和驱动开销。
深度内存估算按 D24 常见的 32 bit 存储计算，实际以平台报告为准。

Android 后续验收应使用目标 GPU 对应的 Android AssetBundle 和构建，在同一固定牌组/分辨率下对比三个档位，
记录 CPU/GPU 帧时间、P95/P99、内存峰值及至少 15 分钟热稳定性。分别覆盖列表滚动、详情拖拽和战斗同屏；
桌面 GPU 的结果不能代替这些测试。不需要制作第二套美术源文件，但平台构建仍必须生成 Android 资源包。

## 参考

- [Unity 2019.4 Quality Settings](https://docs.unity3d.com/2019.4/Documentation/Manual/class-QualitySettings.html)：以预设组合集中管理质量参数。
- [RenderTexture.Release](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/RenderTexture.Release.html)：显式释放渲染纹理硬件资源。
- [Profiler.GetRuntimeMemorySizeLong](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/Profiling.Profiler.GetRuntimeMemorySizeLong.html)：Unity 对象本机内存统计边界。
- [FrameTimingManager](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/FrameTimingManager.html)：平台提供的帧计时。

## 实测记录

2026-09-19，Unity 2019.4.41f2 Windows Development Player，D3D11。
固定窗口尺寸、24 张小卡 + 1 张大图，无 VSync/帧率上限；每段采样 12 秒。
公开记录已省略测试电脑的 CPU/显卡型号和窗口分辨率；以下数据仅用于同机各档位对比，
不作为可跨设备直接比较的硬件基准。
后台窗口运行，卡片通过生产 Camera.Render 路径实际渲染到 RT。
**下表 FPS 是专项循环吞吐，不是前台完整游戏或 Android 手机帧率。**
特别是高档大图逐帧绘制，在这个无上限测试中达到 539–606 次/秒，不能把这种收益比例套到锁 30/60 FPS 的手机。

脱敏记录：[windows-2026-09-19.json](Validation/DynamicCardQuality/windows-2026-09-19.json)。

| 档位/轮次 | 循环 FPS | 平均帧耗时 ms | P95 ms | P99 ms | 相机 CPU 提交 ms/帧 | 持久卡片 RT MiB |
|---|---:|---:|---:|---:|---:|---:|
| 高 / 正序 | 539.29 | 1.854 | 3.554 | 7.053 | 0.628 | 35.00 |
| 中 / 正序 | 880.81 | 1.135 | 1.795 | 2.103 | 0.159 | 16.50 |
| 低 / 正序 | 963.77 | 1.038 | 1.610 | 2.021 | 0.103 | 8.75 |
| 低 / 倒序 | 942.38 | 1.061 | 1.638 | 2.085 | 0.106 | 8.75 |
| 中 / 倒序 | 898.77 | 1.113 | 1.763 | 2.019 | 0.154 | 16.50 |
| 高 / 倒序 | 605.62 | 1.651 | 2.330 | 3.206 | 0.607 | 35.00 |

可以确认的结果：

- 中档持久卡片 RT 比高档减少 **52.9%**，低档减少 **75.0%**；低档相对中档再减少 **47.0%**。
- 每张小卡实际重绘频率分别约 24 / 15 / 10 Hz，中低档大图均约 30 Hz；25 个视图全程有连续重绘，没有轮流分配播放名额。
- 中低档相机 CPU 提交成本和循环帧耗时在正、倒序两轮中都下降。低档比中档有进一步收益，而非只换档位名称。
- Unity allocated memory 约 445.2 MiB，各档基本相同；它不能反映独立 GPU 上的 RT 显存差异，也不代表源素材已缩小。
- 此 API/运行环境没有返回有效 GPU 时间，报告 `meanGpuMs=-1`；没有测得整进程专属显存或 Android 热稳定性。

功能验证 **200 项检查通过，errors=[]**：旧开关迁移、异常值回退、设置持久化、四语言选择器、与整体画质事件隔离、重新打开设置保留档位、150 次逐视图模型/动画时间检查、每档尺寸与连续重绘、关闭清空 RT/模型/包、快速开关取消加载、关闭后重新启用全部 25 张卡、全局分辨率和质量级别不变。
Windows 完整开发构建与最终脚本增量构建均成功，26 个现有资源载荷文件打入测试包；测试后恢复编辑器原场景和项目设置。

同一动画时刻的高/中/低原始 RT 图保存在 `work/DynamicCards/QualityBenchmark/Standalone/same-frame-*.png`。
人工检查高、低对比：主体姿态、火焰及粒子保留，低档细节更软；原 RT 的边缘填充区也保留，不将它误认为新档位产生的 UI 裁切问题。
这项检查只覆盖选定样本，不等同于 677 张卡全部逐帧验收。

早期编辑器 8K 测试受其他 GPU 工作负载干扰，未用于上表结论。
两次预备独立包测试分别遇到后台整屏截图/ReadPixels 的引擎限制；已去掉整屏截图，仅保留显式卡片 RT 对比图，最终复跑为 PASS。
没有 Android 真机结果，因此当前结论是「实现与桌面专项成本下降已验证」，不是「所有手机的中低档均已调到最佳」。
