# Unity 编辑器升级记录（2019.4.1f1 → 2019.4.41f2）

本文件记录 `diy-ai` 分支上 Unity 客户端编辑器版本的升级、随附的包管理器（UPM）变更、权威来源，以及本地打开/构建与 CI 的前置条件。这是源码、脚本与文档的一致性补齐，不是一次新的引擎升级。

English summary: the `diy-ai` Unity client moved from editor `2019.4.1f1` (`e6c045e14e4e`) to `2019.4.41f2` (`6b23d448b533`). This note records the authoritative files, the key UPM changes, local/CI prerequisites, and the verification boundary. It makes no claim that a full client or Android build, or current end-to-end gameplay, has passed on this branch.

## 版本对照

| 项目 | 升级前 | 升级后 |
| --- | --- | --- |
| Unity 编辑器 | 2019.4.1f1 | 2019.4.41f2 |
| 修订号 revision | e6c045e14e4e | 6b23d448b533 |

官方发布说明：<https://unity.com/releases/editor/whats-new/2019.4.41f2>

Windows 编辑器安装器下载地址（2026-09-24 已验证 HTTP HEAD 返回 200；未下载或执行安装器）：

`https://download.unity3d.com/download_unity/6b23d448b533/Windows64EditorInstaller/UnitySetup64-2019.4.41f2.exe`

## 权威来源（source of truth）

- `src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/ProjectSettings/ProjectVersion.txt`
  的 `m_EditorVersion` 与 `m_EditorVersionWithRevision` 是编辑器版本与修订号的唯一来源。
  `scripts/dev-common.ps1` 只在此处解析一次 `UnityVersion` / `UnityRevision`，
  `scripts/setup-dev.ps1`（安装器名称/下载地址）与 `scripts/open-unity.ps1`
  （编辑器路径与提示信息）复用同一结果，不再各自硬编码版本号。
- `src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Packages/manifest.json` 与
  `Packages/packages-lock.json` 是 UPM 依赖的权威来源。

## 关键 UPM 变更

编辑器升级时 Unity 重新解析了包，`git diff upstream/diy-ai...HEAD` 中的主要变化为：

- Addressables 1.18.11 → 1.18.19（传递依赖 scriptablebuildpipeline 1.19.1 → 1.19.2）
- Ads 3.4.7 → 3.7.5；Analytics 3.3.5 → 3.6.12
- Collab Proxy 1.2.16 → 1.14.18；Rider 1.1.4 → 1.2.1；VS Code 1.2.1 → 1.2.5
- 新增 `com.unity.ide.visualstudio` 2.0.15
- Newtonsoft Json 3.0.1 → 3.0.2；Test Framework 1.1.14 → 1.1.31；ext.nunit 1.0.0 → 1.0.6
- TextMeshPro 2.0.1 → 2.1.4；Timeline 1.2.6 → 1.2.18
- Purchasing 2.0.6 → 4.1.5，并新增传递依赖 `com.unity.services.core` 1.0.1
- XR Legacy Input Helpers 2.1.4 → 2.1.9；multiplayer-hlapi 1.0.6 → 1.0.8
- UGUI 依赖新增 `com.unity.modules.imgui`

未改变的边界：Unity 侧 SignalR 客户端仍为 5.0.8（CI 校验和保护该程序集），Common/AI 仍为 `netstandard2.0`，服务端端口、配置文件与运行行为不变。

## 本地打开与构建前置条件

- 脚本管理的安装方式：运行 `scripts/setup-dev.ps1`，它会按 `ProjectVersion.txt`
  解析出的版本/修订号，将编辑器安装到
  `%LOCALAPPDATA%\LegacyGwentDev\Unity\2019.4.41f2`。通过 Unity Hub 激活许可证。
  然后使用 `scripts/open-unity.ps1` 打开项目；它会先重编并同步
  `Assets/Assemblies/Cynthia.Card.Common.dll`。
- 若已经通过 Unity Hub 安装 **2019.4.41f2**，可从 Hub 直接打开项目。
  `open-unity.ps1` 当前只查找上述脚本管理目录，不会自动发现 Hub 的其他安装路径。
- 平台模块：基础编辑器即可打开项目并在编辑器中运行。
  构建 Windows 独立播放器需要 Windows Build Support；构建 Android 需要
  Android Build Support（含 SDK/NDK/JDK，项目为 Android 选择 IL2CPP）。
  Unity Hub 的模块选择与 CI 镜像预置的模块可能不同，缺失模块会导致对应目标构建失败。

## CI 现状

- 工作流 `release.yml`、`unity-build-desktop.yml`、`unity-build-mobile.yml` 使用
  `projectPath: src/Cynthia.Card.Unity/src/Cynthia.Unity.Card`。
- 这些工作流**没有**显式指定旧的 `unityVersion` 参数，因此 GameCI 依据项目内的
  `ProjectSettings/ProjectVersion.txt` 选择编辑器版本；本地脚本也遵循同一来源。

## 验证边界

本记录核对了源码、脚本与文档中的“当前版本”与 `ProjectVersion.txt` 的一致性。尚未对最终候选版本完成完整的客户端构建、Android 构建或新旧客户端端到端对局验证；这些仍需由构建流程与真机/对局回归独立确认。已有历史构建或测试记录不替代最终候选验收。
