# Contribution to LegacyGwent

[简体中文](CONTRIBUTING.md) | [English](CONTRIBUTING_EN.md)

## Environment

### 1. Operation System

Windows, macOS, Linux all Good.

### 2. Server: .NET Core 3.1 (or the latest version)

Download in [Microsoft Website](https://dotnet.microsoft.com/download/dotnet/3.1)

### 3. Database: MongoDB 4.2 (the exact version)

Installation：

- Method 1（recommended）：windows: use [scoop](https://scoop.sh/)；Mac: use `brew`; Linux: use the package manager
- Method 2：Download in [mongo website](https://docs.mongodb.com/manual/administration/install-community/). But might be buggy.

### 4. Client: Unity 2019.4.1f1 (the exact version)

Installation：

- Method 1（recommended）：install with Unity hub
- Method 2：Download the Exactly version on the Unity website

### 5. Recommended Editor: Visual Studio Code

Download the latest `Visual Studio Code` and install C# plugin：`C# for Visual Studio Code (powered by OmniSharp)`.

In Windows, set `MSBuildSDksPath` in Environment Variable to be the installation path of .NET SDK (such as `C:\Program Files\dotnet\sdk\3.1.201\Sdks`). No need for other OS.

### 6. Other Tools

Download [Git](https://git-scm.com/downloads)

In Windows, we recommend `git-bash` instead of `cmd`. `windows terminal` or `wsl` is also OK.

Git Tutorial：[Learn Git Branching](https://learngitbranching.js.org/?locale=zh_CN)

---

## Development

### Fork

Fork [Original Project](https://github.com/LegacyGwent/LegacyGwent) in GitHub, clone the fork to local, and check out a new branch

```bash
# clone the project，replace <fork address> with your fork address
git clone <fork address>

# cd to the folder
cd LegacyGwent

# switch branch to diy branch
git checkout diy

# Checkout to new branch, replace <newBranchName> with costumer name, such as mydiy
git checkout -b <newBranchName>

# after some commits, sync the local branch to remote branch
git push -u origin <newBranchName>
```

After create your own branch, you can send a Pull Request:

1. Create a Draft Pull Request and include what you want to change in the title. A `[WIP]` shall be included in the title to mark this is a work in progress.
2. When your development is finished, delete the `[WIP]`, convert the Draft PR into Normal PR, and request a reviewer to review.
3. If there is no (too much) problem, the PR can be merged!.

### Running Server

- Method 1 Running locally

In the terminal, go to the project folder

```bash
cd LegacyGwent
```

Build the project first:

```bash
sh scripts/refresh.sh
```

Run database:

```bash
mongod --port 28020
```

After running `mongod` command, It's normal that nothing is printed. It's good! Keep it running and do not stop it.

At the same time, open another terminal，run the Server side：

```bash
dotnet watch --project src/Cynthia.Card/src/Cynthia.Card.Server/Cynthia.Card.Server.csproj run
```

- Method 2 Running in docker

In the terminal, go to the project folder

```bash
cd LegacyGwent
```

Running the Docker container

```bash
docker-compose up -d
```

That's it!

><font color=red>__Note!__</font>
>
>If you change anything in `src/Cynthia.Card/src/Cynthia.Card.Common`, you need to rebuild the project, then copy `src/Cynthia.Card/src/Cynthia.Card.Common/bin/Debug/netstandard2.0/Cynthia.Card.Common.dll` to `src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Assemblies/Cynthia.Card.Common.dll`.
>
>`scripts/refresh.sh` (in `diy` branch) can help you rebuild the project and the copy-paste.

### Build the client in Unity

You can change the IP address in `src/Cynthia.Card.Unity/src/Cynthia.Unity.Card/Assets/Script/LoginScript/Bootstrapper.cs` to link to different server. `localhost` or `127.0.0.1` is the local computer.

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

Then use Unity to open `LegacyGwent/src/Cynthia.Card.Unity/src/Cynthia.Unity.Card`. Now you can select `build` to build a client that links to the local server.

---

### Modify Cards

In `LegacyGwent/src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects` you can see the implementations of all cards.

When the server is running，if you change the card effect, the server will automated restart and apply the new modification. If you are in a match when restarting, you need to reopen the client to avoid some bugs.

### Add Cards

The ID of DIY cards starts from 70001. The picture ID of cards uses the name of pictures [here](https://github.com/neal2018/GwentResource/tree/master/FromHC/formatted_only_hc).（without `.png`）.

To add a new card：

1. Add information in `src/Cynthia.Card/src/Cynthia.Card.Common/GwentGame/GwentMap.cs`.

2. Add information in `src/Cynthia.Card/src/Cynthia.Card.Common/GwentGame/CardId.cs`.

3. Implement the card effect in `LegacyGwent/src/Cynthia.Card/src/Cynthia.Card.Common/CardEffects/DIY`.

Good Luck! If you have any questions, you can ask in the community or raise an issue.
