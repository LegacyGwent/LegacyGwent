# 巫师之昆特牌 怀旧版 参与开发流程

[简体中文](CONTRIBUTING.md) | [English](CONTRIBUTING_EN.md)

[参考文档](CardCodingTutorial.md)

## 开发环境

### 1. 操作系统

Windows, macOS, Linux均可。

### 2. 客户端与服务端后台: .NET Core 3.1（最新版即可）

直接在[微软开发者官网](https://dotnet.microsoft.com/download/dotnet/3.1)下载对应版本然后安装即可。

### 3. 客户端UI: Unity 2019.4.1f1（版本必须一样）

安装方法：

- 方法一（推荐）：使用Unity hub安装。
- 方法二：在Unity官网直接下载对应版本安装。

### 4. 数据库: MongoDB 4.2 （版本必须一样）

安装方法：

- 方法一（推荐）：windows环境下使用 [scoop](https://scoop.sh/) 安装；Mac环境下使用 `brew` 安装，linux使用自带的包管理器安装。
- 方法二：在[mongo官网](https://docs.mongodb.com/manual/administration/install-community/)下载最新版本安装。不过这个方法会可能遇到比较多问题，比如安装进度条不动等，试试百度解决。

### 5. 推荐编辑器：Visual Studio Code

下载最新版的 `Visual Studio Code`， 安装C#插件：`C# for Visual Studio Code (powered by OmniSharp)`。

Windows环境下，建议在环境变量中把MSBuildSDksPath设置为安装的 .NET sdk的路径（如 `C:\Program Files\dotnet\sdk\3.1.201\Sdks`）。

### 6. 其他工具

下载并安装[git](https://git-scm.com/downloads)。`terminal` 建议使用 `git-bash`，不要使用 `cmd` 。熟练的玩家也可以用 `windows terminal` 或者 `wsl` 进行开发。

Git教程推荐：[Learn Git Branching](https://learngitbranching.js.org/?locale=zh_CN)，推荐学习英文版（右下角可调语言）。

---

## 开发流程

### Fork项目

在GitHub fork[原项目](https://github.com/LegacyGwent/LegacyGwent)后，将自己的fork克隆到本地，然后创建自己的分支。

```bash
# clone fork的项目，<fork项目地址>用自己的fork项目地址代替
git clone <fork项目地址>

# 移到项目目录
cd LegacyGwent

# 分支切换到diy分支
git checkout diy

# 创建新的分支， <新分支名称>用自己的分支名代替，如 mydiy
git checkout -b <新分支名称>

# 把本地的改动同步到自己的fork的github项目里
git push -u origin <新分支名称>
```

><font color=red>__注意！__</font>
>
>上述操作完毕后，建议先进行下文的编译服务端和编译Unity，确定本地环境没有问题后再进行开发！

创建自己的分支完毕后，可以对原项目创建Pull Request，具体步骤为：

1. 创建Draft Pull Request，标题写明自己想要修改的内容，标题带有`[WIP]`前缀，表明开发正在进行。
2. 开发完毕后，删除`[WIP]`前缀，把Draft PR转换为普通的PR，然后在Pull Request中向项目维护者申请审核。
3. 没有太大问题的话，就可以合并啦。

---

### 编译和运行服务端
- 方法一：本地运行

在terminal中，cd 到`LegacyGwent/`下

```bash
cd LegacyGwent
```

先进行编译：

```bash
sh scripts/refresh.sh
```

运行数据库：

```bash
mongod --port 28020
```

mongod命令执行后，如果没有任何反应和输出，这是正常的！不要停止这个命令。

同时新开一个terminal，运行服务端：

```bash
dotnet watch --project src/Cynthia.Card/src/Cynthia.Card.Server/Cynthia.Card.Server.csproj run
```
- 方法二：使用docker运行服务器

在terminal中，cd 到`LegacyGwent/`下

```bash
cd LegacyGwent
```

运行服务器容器

```bash
docker-compose up -d
```

><font color=red>__注意！__</font>
>
>修改了`src/Cynthia.Card/src/Cynthia.Card.Common`文件夹内的文件，如GwentMap.cs后，需要重新进行编译，然后把 `src/Cynthia.Card/src/Cynthia.Card.Common/bin/Debug/netstandard2.0/Cynthia.Card.Common.dll` 文件拷贝到 `src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Assemblies/Cynthia.Card.Common.dll` 。
>
>如果你觉得太麻烦，`scripts/refresh.sh` 文件可以帮你完成重新进行编译和拷贝的动作。
>
>重新编译完服务端后，Unity也需要再次build新的客户端。
>
>其他情况下，不需要重新编译服务端。

### 编译本地Unity，启动启动器

可以修改 `src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Script/LoginScript/Bootstrapper.c` 中客户端连接的服务器地址，使得客户端连接到本地服务器上，localhost为本地。相关代码为：

```diff
public void Awake()
{
    if (DependencyResolver.Container != null)
        return;
    var IP = Dns.GetHostEntry("cynthia.ovyno.com").AddressList[0];
    var builder = new ContainerBuilder();
    builder.Register(x => DependencyResolver.Container).SingleInstance();
-   builder.Register(x => new HubConnectionBuilder().WithUrl($"http://{IP}:5005/hub/gwent").Build()).Named<HubConnection>("game").SingleInstance();
+   builder.Register(x => new HubConnectionBuilder().WithUrl("http://localhost:5005/hub/gwent").Build()).Named<HubConnection>("game").SingleInstance();

    DependencyResolver.Container = AutoRegisterService(builder).Build();
}
```

然后用Unity打开 `LegacyGwent/src/Cynthia.Card.Unity/src/Cynthia.Unity.Card` 项目，选择 `build`，就可以build出连接本地服务器的客户端。

然后，你就可以使用Unity里的编辑器和客户端进行自我对战测试了。祝你好运！

---

### 卡牌修改

在 `LegacyGwent/src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects` 目录下查看卡牌的实现，对卡牌的效果进行修改。

服务端运行时，卡牌效果变动后本地服务器会自动重新启动，不需要重新编译Unity项目，也不需要重新编译服务端，就能在本地Unity编译出的游戏里使用修改后的卡牌。但此时游戏会掉线，如果是在对战中掉线，则需要重新启动游戏，否则会出现对局中无法显示卡牌Bug。

### 卡牌增加

DIY的卡牌序号从70001开始。卡图编号使用[这里](https://github.com/neal2018/GwentResource/tree/master/FromHC/formatted_only_hc) 的图片名（不含.png）。

增加一张卡需要：

1. 在 `src/Cynthia.Card/src/Cynthia.Card.Common/GwentGame/GwentMap.cs` 中加入相应的资料
2. 在 `src/Cynthia.Card/src/Cynthia.Card.Common/GwentGame/CardId.cs` 中加入对应的一行
3. 在 `LegacyGwent/src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects/DIY` 目前下新增相关卡牌效果。

如果还有其他问题，欢迎在群里面提问！
