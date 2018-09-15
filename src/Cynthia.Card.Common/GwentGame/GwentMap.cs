using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card
{
    public static class GwentMap
    {
        public static IDictionary<string, GwentCard> CardMap { get; } = new Dictionary<string, GwentCard>
        {
            //测试卡
            {"tl",new GwentCard(){Strength=18,Group=Group.Leader,Faction = Faction.Nilfgaard,Name="基础领袖",CardUseInfo = CardUseInfo.MyRow,CardType = CardType.Unit}},
            {"tg1",new GwentCard(){Strength=17,Group=Group.Gold,Faction = Faction.Neutral,Name="基础金卡一号",CardUseInfo = CardUseInfo.MyRow,CardType = CardType.Unit}},
            {"tg2",new GwentCard(){Strength=16,Group=Group.Gold,Faction = Faction.Neutral,Name="基础金卡二号",CardUseInfo = CardUseInfo.MyRow,CardType = CardType.Unit}},
            {"tg3",new GwentCard(){Strength=15,Group=Group.Gold,Faction = Faction.Neutral,Name="基础金卡三号",CardUseInfo = CardUseInfo.MyRow,CardType = CardType.Unit}},
            {"tg4",new GwentCard(){Strength=0,Group=Group.Gold,Faction = Faction.Neutral,Name="基础金卡法术一号",CardUseInfo = CardUseInfo.AnyPlace,CardType = CardType.Special}},
            {"ts1",new GwentCard(){Strength=13,Group=Group.Silver,Faction = Faction.Neutral,Name = "基础银卡一号",CardUseInfo = CardUseInfo.MyRow,CardType = CardType.Unit}},
            {"ts2",new GwentCard(){Strength=12,Group=Group.Silver,Faction = Faction.Neutral,Name = "基础银卡二号",CardUseInfo = CardUseInfo.MyRow,CardType = CardType.Unit}},
            {"ts3",new GwentCard(){Strength=11,Group=Group.Silver,Faction = Faction.Neutral,Name = "基础银卡三号",CardUseInfo = CardUseInfo.MyRow,CardType = CardType.Unit}},
            {"ts4",new GwentCard(){Strength=0,Group=Group.Silver,Faction = Faction.Neutral,Name = "基础银卡法术一号",CardUseInfo = CardUseInfo.EnemyPlace,CardType = CardType.Special}},
            {"ts5",new GwentCard(){Strength=9,Group=Group.Silver,Faction = Faction.Neutral,Name = "基础银卡五号",CardUseInfo = CardUseInfo.MyRow,CardType = CardType.Unit}},
            {"ts6",new GwentCard(){Strength=8,Group=Group.Silver,Faction = Faction.Neutral,Name = "基础银卡六号",CardUseInfo = CardUseInfo.MyRow,CardType = CardType.Unit}},
            {"tc1",new GwentCard(){Strength=7,Group=Group.Copper,Faction = Faction.Neutral,Name = "基础铜卡一号",CardUseInfo = CardUseInfo.MyRow,CardType = CardType.Unit}},
            {"tc2",new GwentCard(){Strength=6,Group=Group.Copper,Faction = Faction.Neutral,Name = "基础铜卡二号",CardUseInfo = CardUseInfo.MyRow,CardType = CardType.Unit}},
            {"tc3",new GwentCard(){Strength=5,Group=Group.Copper,Faction = Faction.Neutral,Name = "基础铜卡三号",CardUseInfo = CardUseInfo.MyRow,CardType = CardType.Unit}},
            {"tc4",new GwentCard(){Strength=4,Group=Group.Copper,Faction = Faction.Neutral,Name = "基础铜卡四号",CardUseInfo = CardUseInfo.MyRow,CardType = CardType.Unit}},
            {"tc5",new GwentCard(){Strength=0,Group=Group.Copper,Faction = Faction.Neutral,Name = "基础铜卡法术一号",CardUseInfo = CardUseInfo.MyPlace,CardType = CardType.Special}},
            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //正式卡
            //-----------------------------------------------------------------
            //中立金
            {
                "20015400",
                new GwentCard()
                {
                    Name="皇家谕令",
                    Strength=0,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "吾，弗尔泰斯特，以泰莫利亚圣君、索登亲王、布鲁格守护者及其他各种合法称号之名义，作出如下判决……。",
                    Info = "从牌组打出1张金色单位牌，使其获得2点增益。",
                    CardEffectIndex = "20015400",

                }
            },
            {
                "11210200",
                new GwentCard()
                {
                    Name="杰洛特：伊格尼法印",
                    Strength=5,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "猎魔人晃晃手指就能点灯，或把敌人烧成灰。",
                    Info = "若对方某排总战力不低于25点，则摧毁其上所有最强的单位。",
                    CardEffectIndex = "11210200",

                }
            },
            {
                "20177600",
                new GwentCard()
                {
                    Name="丹德里恩：传奇诗人",
                    Strength=5,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "文胜于武，笔胜于剑。",
                    Info = "抽一张牌，随后打出1张牌。",
                    CardEffectIndex = "20177600",

                }
            },
            //-------------
            //中立银
            {
                "11221000",
                new GwentCard()
                {
                    Name="萝卜",
                    Strength=4,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "杰洛特，我们得来场人马间的对话。恕我直言，你的骑术……真的有待提高，伙计。",
                    Info = "己方每次从手牌打出金色单位牌时，召唤此单位至随机排。",
                    CardEffectIndex = "11221000",

                }
            },
            {
                "11221300",
                new GwentCard()
                {
                    Name="乞丐王",
                    Strength=5,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "要是我缺鼻子或者断手了，那显然，乞丐王接受这两种付款方式。",
                    Info = "若打出后总战力低于对方，则获得强化，直至战力持平或最多15点。",
                    CardEffectIndex = "11221300",

                }
            },
            //-------------
            //中立铜
            {
                "11340200",
                new GwentCard()
                {
                    Name="侦查",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "如果斥候没有回来，我们就掉头。乡下的人说这些树林里全是松鼠。我指的可不是啃松果的那种。",
                    Info = "检视牌组中2张铜色单位牌，随后打出1张。",
                    CardEffectIndex = "11340200",

                }
            },
            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //帝国领袖
            {
                "16110300",
                new GwentCard()
                {
                    Name="约翰·卡尔维特",
                    Strength=5,
                    Group=Group.Leader,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "剑只是统治者的工具之一。",
                    Info = "检视牌组顶端3张卡牌，打出一张。",
                    CardEffectIndex = "16110300",

                }
            },
            //-----------
            //帝国金卡
            {
                "20003200",
                new GwentCard()
                {
                    Name="亚特里的林法恩",
                    Strength=5,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "辛特拉沦陷后，亚特里随即告破，这里的守军若不听任尼弗迦德人驱策，就只能去死。",
                    Info = "从牌组打出一张铜色/银色间谍单位牌",
                    CardEffectIndex = "20003200",

                }
            },
            //----------
            //帝国银卡
            {
                "16221100",
                new GwentCard()
                {
                    Name="约阿希姆·德·维特",
                    Strength=5,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "即便用 “无能” 来形容德·维特公爵对维登集团军的领袖，都算给他面子。",
                    Info = "间谍。\n从牌组顶端打出1张铜色/银色非间谍单位牌，并使它获得10点增益。",
                    CardEffectIndex = "16221100",

                }
            },
            {
                "16221200",
                new GwentCard()
                {
                    Name="冒牌希里",
                    Strength=8,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "“她来了，” 他心想 “皇帝的联姻对象。冒牌公主、冒牌的辛特拉女王、冒牌的雅鲁加河口统治者，也是帝国未来的命脉。”",
                    Info = "间谍。\n回合开始时，若为间谍，则获得1点增益。所在半场的玩家放弃跟牌后，移至对方半场同排。\n遗愿：摧毁同排最弱的单位",
                    CardEffectIndex = "16221200",

                }
            },
            {
                "16240100",
                new GwentCard()
                {
                    Name="魔像守卫",
                    Strength=11,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "石拳阻挡刀剑，逻辑战胜谎言。",
                    Info = "将一个 “次级魔像守卫” 置于对方牌组顶端。",
                    CardEffectIndex = "16221200",

                }
            },
            {
                "16221000",
                new GwentCard()
                {
                    Name="坎塔蕾拉",
                    Strength=13,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "男人渴望着诱惑，神秘加优雅往往事半功倍。",
                    Info = "力竭：抽两张牌。保留一张，将另一张置于牌组底端。",
                    CardEffectIndex = "16221000",

                }
            },
            //------------
            //帝国铜卡
            {
                "16230400",
                new GwentCard()
                {
                    Name="维可瓦罗医师",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "这个世界的瘟疫跟战争一样多，夺人性命也一样出其不意。",
                    Info = "从对方墓地复活1个铜色单位。",
                    CardEffectIndex = "16230400",

                }
            },
            {
                "16231400",
                new GwentCard()
                {
                    Name="特使",
                    Strength=2,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "但是……这样不对啊！两国交兵不斩来使！。",
                    Info = "间谍。\n随机检视牌组中2张铜色单位牌，打出一张。",
                    CardEffectIndex = "16231400",

                }
            },
            {
                "16230700",
                new GwentCard()
                {
                    Name="近卫军",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "近卫军决不投降，绝不。",
                    Info = "场上每有1个敌军间谍单位，便获得2点增益。每有1个间谍单位出现在对方半场，便获得2点增益。",
                    CardEffectIndex = "16230700",

                }
            },
            {
                "16231800",
                new GwentCard()
                {
                    Name="尼弗迦德骑士",
                    Strength=12,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "出自名门望族，生于金塔之城，组成帝国的精锐部队。",
                    Info = "随机揭示1张己方手牌（优先级为铜银金）。\n2点护甲。",
                    CardEffectIndex = "16230700",

                }
            },
        };
        public static IEnumerable<GwentCard> DeckChange(IEnumerable<string> deck)
        {
            var step1 = deck.Select(x => CardMap[x]);
            return step1.Select(x => new GwentCard()
            {
                Categories = x.Categories,
                Faction = x.Faction,
                Flavor = x.Flavor,
                Group = x.Group,
                Info = x.Info,
                Name = x.Name,
                Strength = x.Strength
            });
        }
        public static IDictionary<Group, string> FlavorMap { get; } = new Dictionary<Group, string>
        {
            { Group.Leader, "领袖" },
            { Group.Gold,"金" },
            { Group.Silver,"银" },
            { Group.Copper,"铜" }
        };
    }
}