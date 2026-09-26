# AI 分线客户端打包与交付

检查与修复日期：2026-09-20。最初审计基线为 AI 线首次兼容性移植提交；本文描述修复后的流程。

远端分支、素材清单、原工作区遗漏及 CI 配置的再次核对见
[最终交付核对](AI-DeliveryFinalCheck.md)。

## 四种目标产物

| 平台 | standard | premium |
| --- | --- | --- |
| Windows x64 | 普通卡界面，不携带闪卡动画包 | 闪卡界面和 Windows 专用动画包 |
| Android | 普通卡界面，不携带闪卡动画包 | 闪卡界面和 Android 专用动画包 |

同一套代码、账号和通信接口，通过 `LegacyClientBuild.Build` 的
`-clientVariant standard/premium` 选择内容。无需维护两套客户端代码。
老客户端仍走原有协议；服务器奖励与玩家使用哪种包无关。

## 五项修复

1. **素材交付**：`build-config/premium-content.json` 固定源素材版本和每卷 SHA-256。
   677 张卡的 prefab、贴图、材质、动画、音频及原始 `.meta` 通过 GitHub Release
   分卷交付，避免把约 33.8 GB 的源文件直接塞进普通 Git 历史。
   33 卷 ZIP 合计 5,409,871,278 字节。较大的卷再拆为传输片段，
   当前共 62 个远端文件；恢复时同时验证片段哈希与重组后 ZIP 的哈希。原始素材中已经不存在的音频
   `Latest/Audio/533473923.wav` 从卡 `13860101` 的索引中清除；不伪造音频。
2. **构建入口和矩阵**：desktop、mobile、release 接入 standard/premium，
   Windows 与 Android 均有两种产物名。统一入口先同步 Common DLL（workflow）、
   准备当前平台的动态包，再生成 Addressables 和 Player；finally 恢复临时资源。
   `client-content.json` 写入运行时 Resources 和产物内 StreamingAssets。
3. **标准包界面**：普通包隐藏闪卡筛选、合成、粉尘、日常奖励和动态画质入口，
   只显示普通卡图；不会主动请求闪卡钱包。标准包提交卡组时省略闪卡选择字段，
   让服务器保留同一账号在闪卡客户端的外观选择。切换包不会擦除画质偏好。
4. **Android 架构和资源**：显式 IL2CPP、ARMv7 + ARM64、Android API 21、GLES3。
   Android 动态 PNG 纹理使用 ETC2 RGBA8，最大 1024，不另存一套低清源图。
   Android 使用独立的平台资源包；不支持的后处理 shader 跳过，运行时图形能力
   不足时回退静态卡。低/中/高档仍控制渲染尺寸和刷新频率，不能代替内存实测。
5. **固定安卓签名**：mobile/release 使用同一组 `ANDROID_*` Secrets；缺任一项
   就停止。产物检查同时验证包名、版本、ARM64、签名证书 SHA-256 和资源成员。
   两种内容包保持同一应用身份。新签名不能直接覆盖使用其他签名的旧 APK；
   这次迁移通常要先卸载旧包，本地设置可能丢失，服务器账号数据不会因此被删除。

## 从干净 checkout 恢复素材

标准包无需下载闪卡源素材。闪卡包在仓库根目录运行：

```sh
python scripts/premium-content.py restore
```

脚本验证 ZIP 字节数、SHA-256、文件数、路径和 catalog，先暂存后安装，
拒绝覆盖已有用户素材。只允许保留 checkout 中的 catalog/meta；meta 的
换行及行尾空格差异被容忍，GUID 等实际内容不同则停止。catalog 由 Git
属性固定换行格式，以保持 Windows/Linux checkout 的哈希一致。
下载缓存默认 `.premium-downloads`，不提交 Git。离线可传 `--cache` 和 `--local-only`。
需要源文件解压空间，还需为 Unity Library、平台包及 Docker 镜像预留足够磁盘；
CI 在恢复 Unity Library 前清理临时 runner 的多余工具和 Android SDK，
保留 APK 检查所需 build-tools/Java；每卷解压成功后删除下载 ZIP，减少约 5.4 GB
累计缓存占用。自托管机器不会自动清除工具。磁盘预检只能检查解压空间，
不能证明完整构建峰值足够。

发布新源版本时使用 `scripts/premium-content.py pack` 更新 manifest，再用
`scripts/publish-premium-content.py` 上传。较大 ZIP 可先执行
`scripts/premium-content.py split-transport --cache <压缩包目录>`。只有全部远端卷的大小和 SHA-256
匹配，草稿素材 Release 才公开；已发布版本禁止修改，变更应使用新 tag。
源素材 tag 不以 `v` 开头，不触发客户端正式发布。

## CI 与签名配置

- 云端 Unity 构建仍需配置 `UNITY_LICENSE`；GameCI 个人版配置还列有
  `UNITY_EMAIL`、`UNITY_PASSWORD`，workflow 已接入这些 Secrets。当前 fork 尚缺
  该配置，workflow 会在下载大素材前明确失败。这不是要求另购许可证，也不表示
  本机 Unity 未激活。现行 [GameCI 说明](https://game.ci/docs/github/activation/)
  支持使用 Hub 生成的个人版 `.ulf` 跨平台配置；本机文档默认位置未找到该文件。
  不要将许可证/密码放入公开仓库或聊天，不要为此中断正在使用的本机激活。
- 安卓需要 `ANDROID_KEYSTORE_BASE64`、`ANDROID_KEYSTORE_PASSWORD`、
  `ANDROID_KEY_ALIAS`、`ANDROID_KEY_PASSWORD`、`ANDROID_CERT_SHA256`。
  这些已经配置到当前 fork，私钥及密码不在 Git 中。
- `scripts/configure-android-signing.py` 可在仓库外生成/复用固定签名并加密上传 Secrets；
  `--upload` 需要 PyNaCl。必须另做离线备份；不要在新机器上随意生成替代密钥。
- 合入上游时使用上游自己的许可证与安卓签名 Secrets；fork 的私钥不会随 PR 传递。
- fork 的部署工作流已加仓库身份限制；推送本分支不会部署上游 AI/DIY 服务。

## 已执行验证及边界

- .NET Release solution 构建：0 警告、0 错误；服务器兼容测试 54 项通过。
- 客户端契约检查 27 项通过，包括普通包不擦除闪卡选择和不覆盖画质偏好。
- 素材/产物交付测试 19 项通过：哈希损坏、目录穿越、meta 冲突、已有文件保护、
  普通包混入动态资源、闪卡分卷缺失、APK 缺 ARM64 等均能被拒绝。
  包含传输片段重组、缺片、损坏片段、读取重试及草稿发布保留标签的检查。
- 33 卷真实素材已在 AI 工作区解压和校验，677 张卡恢复完成。
- [源素材 Release](https://github.com/lwr511/LegacyGwent/releases/tag/premium-source-20260920-v1)
  已公开发布；重新读取远端确认 62 个文件均上传完成，大小和 SHA-256 全部匹配。
  未登录下载抽查 `premium-source-004.zip.part-007`（3,128,221 字节）及哈希通过。
  素材 tag `premium-source-20260920-v1` 指向包含对应 manifest 的源码快照。
- 使用 Unity 2019.4 本地引用做 Roslyn 静态编译：Windows runtime、Android runtime、
  Windows Editor 均通过，Android 额外启用 `ENABLE_IL2CPP` 条件符号也通过。
  这不是 Unity Player、原生 IL2CPP 或 shader 构建。
- workflow YAML、shell 语法及 Git whitespace 检查纳入提交前验证。
- **没有生成或安装本次 APK/Windows Player，也没有安卓真机视觉、内存、温度、
  覆盖升级和实际对局验证。** 本机 Unity 2019 缺 Android 模块；云端缺上述许可证。
  因此修复配置和源码不等于四个真实产物已经验收。

构建后运行 `scripts/verify-client-content.py` 验证嵌入标记和动态包成员；Android
再通过 workflow 的 aapt/apksigner 检查。上线前还要进行旧客户端登录、编辑卡组、
匹配及对局回归，和新闪卡客户端合成/选择/实际对局验证。
