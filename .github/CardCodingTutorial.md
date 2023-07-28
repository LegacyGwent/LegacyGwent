# 新手卡牌编程教程——以“红骑士”为例的0基础使用者视角

---
## 0.前言

笔者作为一个没有任何编程基础的菜鸟选手，因为某些玩家在宣称自己多次尝试写红骑士代码却超过半年无果，并且其无法提供自己失败思路以及心路历程。因此笔者决定以一个新手的视角记录“红骑士”的编程过程。因为我们不知道某些玩家在半年的过程中进行到了哪一步，所以笔者将记录从新建文件夹到提交PR的全过程以供参考。
笔者并非专业程序员，其中必然存在大量谬误以及数不胜数的“知其然不知其所以然”的过程，请海涵见谅！

---
## 1.准备工作
源码下载以及编译请参考[CONTRIBUTING.md](/CONTRIBUTING.md)。

### 你的第一张卡牌
据笔者所知，某玩家在下载源码这一步存在的问题并不大，但是之后的步骤在大半年内似乎再无进展，笔者百思不得其解，后来在阅读倚天屠龙记时豁然开朗。
>看了半晌，见两人出招越来越快，他心下却越来越不明白：“我外公和宋大伯都是武林中一流高手，但招数之中，何以竟存着这许多破绽？外公这一拳倘若偏左半尺，不就正打中宋大伯的胸口？宋大伯这一抓若再迟出片刻，那不恰好拿到了我外公左臂？难道他二人故意相让？

飞禽见地下狮虎搏斗，不免会想：“何不高飞下扑，可制必胜？”只不过力有不逮耳。
因此我们必须先测试一下自己是否具备写一张新卡的能力。
打开**src\Cynthia.Card\src\Cynthia.Card.Common\GwentGame\GwentMap.cs**文件，找到编号13001的卡牌萝卜，将代码块中的**Strength=4**改为**Strength=40**

此时你的第一张卡牌就完成了，你已经将萝卜修改为40点。如果你完成了这一步，就表明你确实是具备了写一张新卡的能力。如果你想测试是否成功，可以按照上一步所说的方法进行编译，然后进行对战测试。如果你的设备没有配置上述所说的环境又或者你不希望本地环境变得乱七八糟，笔者将会在后文中提供docker启动服务器的方法。最后别忘了把杰洛特的点数调整为3，这样会让你对战AI更轻松。

---
## 2.卡牌效果
以“红骑士”为例，卡牌效果为“每当有敌军单位被“刺骨冰霜”摧毁时，从牌组召唤1张它的同名牌。”

观察其效果，一个没有基础的使用者第一步应该做的事情应该是找到一张效果类似的卡牌。笔者观察这张卡牌时第一个想到的是鹰身女妖这张卡牌、

因此我们选择鹰身女妖作为参考基准。

### 文件路径
卡牌效果的文件路径为**src\Cynthia.Card\src\Cynthia.Card.Common\CardEffects**

找到基准卡牌

鹰身女妖**src\Cynthia.Card\src\Cynthia.Card.Common\CardEffects\Monsters\Copper\Harpy.cs**

### 新建文件
找到基准卡牌后，建立自己的卡牌效果文件了
路径为**src\Cynthia.Card\src\Cynthia.Card.Common\CardEffects\DIY\Monsters\Copper**
“DIY\Monsters\Copper”表示其为DIY卡牌、怪兽阵营、铜色品质。
在该文件夹新建文件**RedRider.cs**
文件名应为该卡牌英文名，每个单词首字母大写，以做规范。

### 抄袭代码
现在你已经新建了新卡的文件了，制作一张新卡的核心任务已经完成了，剩下的就是简单的抄袭工作了！

#### 抄袭框架
将鹰身女妖的代码复制进RedRider.cs文件中，你的择一框架就此完成了！

#### 卡牌的自主产权
将卡牌中的"Harpy"全部替换为"RedRider"。完成了这一步，红骑士这张卡牌就完全成为我们原创的卡牌了！
（稍后还有CardId的修改，在实际操作中这里可以一并完成，但是为了教程的流程，该步骤放在后面再说。）

#### 观察卡牌结构
这一步未必科学，也未必正确，但是是一个让新手快速了解C#代码结构的好方法！
##### 引用命名空间

```
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
```
该结构一般是固定的，不需要修改

##### Card命名空间

```
namespace Cynthia.Card
{

}
```
所有的卡牌应放在该命名空间内，该结构一般是固定的，，不需要修改

##### 新建卡牌的效果方法
```
[CardEffectId("24030")]//鹰身女妖
    public class Harpy : CardEffect, IHandlesEvent<AfterCardDeath>
    {//每当1个友军“野兽”单位在己方回合被摧毁，便召唤1张同名牌。
        public Harpy(GameCard card) : base(card) { }
        public async Task HandleEven(AfterCardDeath @event)
        {
        }
    }
```
该结构是我们主要需要改的地方CardEffectId，与该卡牌的ID相同，这取决于你给该卡牌的编号。应当按照DIY卡牌顺序，在最新的编号后加一。
```
public class Harpy : CardEffect
```
其中Harpy是该卡牌的方法名，应当与文件同名，按照规范应当为该卡牌的英文名、首字母大写。
```
AfterCardDeath @event
```
游戏行为，如文字所示，该行为表示在卡牌死亡后
@event 笔者的理解就是当有卡牌死亡后，我们将那个行为命名为@event。

最后的{}中就是我们需要写自己代码的地方了。

#### 观察卡牌代码
上一步{}中的代码就是卡牌实际运行的代码
```
{
    if (Game.GameRound.ToPlayerIndex(Game) == PlayerIndex && @event.Target.HasAllCategorie(Categorie.Beast) && @event.Target.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsInDeck())
    {
        var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
        if (list.Count() == 0)
        {
            return;
        }
        //只召唤最后一个
        if (Card == list.Last())
        {
            await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), Card);
        }

        return;
    }

    return;
}
```

我们观察该代码第一个if内的内容，具体的内容如何实现我们不太需要关注，但是结合我们的游戏体验，意思应当是从卡组中召唤卡牌。结合代码中的if (list.Count() == 0)，该代码块的作用应当是当卡组中剩余的该卡牌数量为0时，不能再召唤了，因此直接return。
list在上一步的定义格式为var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();

想要理解这行代码，作为初学者，尤其是一名只想写卡牌的初学者，笔者给出的方法叫做“顾名思义”，先了解应该如何使用而非理解其深刻含义。
我们的目标是将己方牌库中所有的鹰身女妖做成一个列表：
* Game表示当前游戏
* PlayersDeck表示玩家的卡组
* Card.PlayerIndex其中Card表示我们这张卡牌，即鹰身女妖。 .PlayerIndex表示获取这张卡牌是属于哪个玩家了。这个参数在PlayersDeck里，表示当前玩家的卡组
* Where是C#中的一个方法
* x，Where中的x表示我们定义一个x，这个x要符合以下条件，即x的id等于Card的id。Card就是鹰身女妖
* ToList，上一步我们已经获取了牌组中所有的鹰身女妖，现在用ToList()方法将其转化为一个列表

if (Card == list.Last())代码块的效果与此类似。这并不是该教程的主要目的，因此不在细说。读者应该结合自己的实践举一反三。

观察这段代码，我们要将其修改成红骑士的代码，最主要的是修改第一行的判断条件
```
if (Game.GameRound.ToPlayerIndex(Game) == PlayerIndex && @event.Target.HasAllCategorie(Categorie.Beast) && @event.Target.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsInDeck())
```

#### 修改判断条件

**红骑士：每当有敌军单位被“刺骨冰霜”摧毁时，从牌组召唤1张它的同名牌。**

**鹰身女妖：每当1个友军“野兽”单位在己方回合被摧毁，便召唤1张同名牌。**

我们对照效果来看一下对应的代码
* Game.GameRound.ToPlayerIndex(Game) == PlayerIndex 这个判断条件表示游戏的回合，鹰身女妖的判断条件是“己方回合”，所以代码中写的是PlayerIndex

红骑士的触发条件是被霜打死，那么既然是被霜打死，那么一定是在对方回合，这里我们需要修改成对方的回合。
那么对方的回合我们应该用什么词呢？
两个思路去获取：
1.找到一个类似卡牌
2.找到相关代码

类似的卡牌，要找判断对方的，这样的卡牌有非常多，任何天气都是这样，比奥克维斯塔。
我们找到希里的代码src/Cynthia.Card/src/Cynthia.Card.Common/RowEffect/BitingFrostStatus.cs
其中有一行var cards = Game.GetAllCard(AnotherPlayer).Where(x => x.PlayerIndex == **AnotherPlayer** && x.Status.CardRow.IsOnPlace());
问题解决，应该用**AnotherPlayer**

第二种方法，我们在所有代码中搜索**GameRound**，发现在src/Cynthia.Card/src/Cynthia.Card.Common/GwentInterface/IGwentServerGame.cs这个叫i昆的文件中搜到TwoPlayer GameRound { get; set; }//谁的的回合----这样的代码，继续往下看，找到int AnotherPlayer(int playerIndex);

**这里笔者当然推荐第一种方法！！！**

另外我们可以采取一个稍微简单点的方法，把代码修改为Game.GameRound.ToPlayerIndex(Game) != PlayerIndex
即不在玩家回合，那么一定就是对手的回合了

* @event.Target.HasAllCategorie(Categorie.Beast)
这里表示的是判断触发事件的卡牌类型是否为野兽。

我们并不需要该判断，直接将该条件删除。但是简单讲解一下。@event我们之前说过了，这是触发的事件。我们打开*src/Cynthia.Card/src/Cynthia.Card.Common/GameEvent/AfterCardDeath.cs*文件，这个事件有两个属性，Target和DeathLocation。Target就是触发该事件的卡牌，它的类型是GameCard；DeathLocation顾名思义就是死亡的位置。
搜索GameCard，找到*src/Cynthia.Card/src/Cynthia.Card.Common/GwentGame/GameCard.cs*我们可以看到它所拥有的属性，包括Status等，你可以进一步了解Status，这里不再阐述。

* @event.Target.PlayerIndex == Card.PlayerIndex

这个条件判断目标是否在己方，同样的，被霜摧毁的目标不在己方，将其按照第一步修改。


* Card.Status.CardRow.IsInDeck()

这个条件判断该卡牌是否在牌库，这里的该卡牌指的是鹰身女妖，我们的红骑士当然也要进行该次判断，因此保留该条件。

其实在下面的判断中，我们也有判断牌库中是否有该卡牌。但是对于新手来说，多一次冗余的判断，并不是太大的问题。

至此，我们的判断条件为
if (Game.GameRound.ToPlayerIndex(Game) == AnotherPlayer && @event.Target.PlayerIndex == AnotherPlayer && Card.Status.CardRow.IsInDeck())

即对方有一张卡牌死亡，并且在对方的回合死亡，并且你有红骑士在自己的牌库中。还缺少什么条件呢？至此还缺少两个条件
* 死亡卡牌所在排有霜
* 死亡卡牌被霜打死

找到一个参考，判断对方是否在天气影响下的卡牌有很多，比如狂猎战士

其判断的代码
```
var isBoost = Game.GameRowEffect[target.PlayerIndex][target.Status.CardRow.MyRowToIndex()].RowStatus == RowStatus.BitingFrost;
```

我们进行一下验证，上一步提到的Status属性，我们去找一下*src/Cynthia.Card/src/Cynthia.Card.Common/Models/CardStatus.cs*文件
其中有一句*CardRow = position;*
搜索RowStatus找到*src/Cynthia.Card/src/Cynthia.Card.Common/GwentGame/CardProperty/RowStatus.cs*里面确实有所有的天气状态。

这个判断就可以判断死亡卡牌是否在霜中了。但是这样会出现一个问题，就是我们假设在对方回合，对方有一个随从在霜中，然后被威爷烧死，这样红骑士依旧会触发。这不符合我们的要求，因此我们还需要加一条判断条件判断伤害是否来源于霜。

但是笔者并没有找到在AfterCardDeath方法中判断伤害来源的方法，这对于笔者来说是一个折磨人的遗憾，但是对于教程来说确实一个千载难逢增加知识点的好机会！

#### 更改事件方法

笔者找了很久，终于找到一个可以判断伤害类型的方法。事件的方法在*src/Cynthia.Card/src/Cynthia.Card.Common/GameEvent*文件夹。其中里面所有的事件都如命所示，非常清晰。
其中有一个AfterCardHurt的方法， 里面有public DamageType **DamageType** { get; set; }伤害类型的判断。

将我们红骑士文件中的AfterCardDeath替换为AfterCardHurt，该事件监视卡牌受到伤害而非卡牌死亡，因此我们需要加一个判断@event.Target.IsDead判断卡牌是否死亡。
现在增加判断伤害类型
**bool isBitingFrost = (@event.DamageType == DamageType.BitingFrost);**
这里判断伤害类型是否为霜，有了伤害类型，我们就可以把卡牌是否在霜中的判断条件去除了

最终完整的判断条件为
```
if (Game.GameRound.ToPlayerIndex(Game) == AnotherPlayer && @event.Target.PlayerIndex == AnotherPlayer && Card.Status.CardRow.IsInDeck() && isBitingFrost && @event.Target.IsDead)
```
卡牌效果至此完成！

---

## 3.结尾工作

正如昆特需要一个完美的卡牌作为收尾，编写卡牌代码的过程也需要一个收尾。不过不必担心，因为你的对手是停留在第0步的某玩家，你并不需要很大的点数，你只需要一些简单而琐碎的工作，让你的必然的胜利显得更加优雅！

以下工作并无任何难度，只是简单的修改，因此我不会演示具体步骤，只作提醒。

### 修改Map文件
在**src/Cynthia.Card/src/Cynthia.Card.Common/GwentGame/GwentMap.cs**文件的最后参考上面的格式添加你新卡的json
***如果增加了新卡，改变了卡图以及卡牌的描述请务必修改MAP文件开头的版本号，加一即可。***

### 添加方法序号
在**src/Cynthia.Card/src/Cynthia.Card.Common/GwentGame/CardId.cs**添加你效果方法的序号

### 添加本地化文本
在**src/Cynthia.Card/src/Cynthia.Card.Server/Locales**文件夹中分别添加各语言的文本

### 修改注释
按照规范修改红骑士的注释，以及修改其ID，这步在实际操作中应该在修效果代码时就已经完成了

---

## 4.Docker服务器的部署
因为不同操作系统的环境不同，安装配置环境也有所不同，因此用docker部署服务器是一个不错的选择。首先根据你的操作系统安装docker，运行docker-compose up -d即可部署服务器
在你修改了代码后，想要重新构建新的镜像，在该命名后加上--build，即docker-compose up -d --build

如果你不想以docker-compose运行，你需要先用docker运行你的数据库，如果不使用docker数据库而使用本地或者远程的数据库可以省略这一点，然后进入**src/Cynthia.Card**目录，手动构建镜像，使用命令
```
docker build -t <image_name> .
```

构建镜像

最后运行
```
docker run -d \
  --name legacygwent-server \
  -p 5005:5005 \
  --restart=always \
  -e MONGO_CONNECTION_STRING=mongodb://{your_mongodb}/gwent-diy \
  <image_name>
```

