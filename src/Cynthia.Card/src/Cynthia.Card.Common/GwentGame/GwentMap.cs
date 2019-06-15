using System;
using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card
{
    public static class GwentMap
    {
        public static IEnumerable<CardStatus> GetCards(bool isHasDerive = false)
        {
            return CardMap
            .Where(x => !x.Value.IsDerive)
            .OrderByDescending(x => x.Value.Group)
            .ThenBy(x => x.Value.Faction)
            .ThenBy(x => x.Value.Strength)
            .Select(x => new CardStatus(x.Key));
        }
        public static IEnumerable<string> GetCreateCardsId(Func<CardStatus, bool> sizer, int count = 3, bool isHasDerive = false)
        {
            return GetCards()
                .Where(sizer)
                .Mess().Take(count).Select(x => x.CardId)
                .ToList();
        }
        public static IEnumerable<string> GetCardsId(bool isHasDerive = false)
        {
            return CardMap
            .Where(x => !x.Value.IsDerive)
            .OrderByDescending(x => x.Value.Group)
            .ThenBy(x => x.Value.Faction)
            .ThenBy(x => x.Value.Strength)
            .Select(x => x.Key);
        }
        public static IDictionary<string, GwentCard> CardMap { get; } = new Dictionary<string, GwentCard>
        {
            //=========================================================================================================================================================================
            //以下是自动导入的代码
            //=========================================================================================================================================================================
            {
                "12004",//利维亚的杰洛特
                new GwentCard()
                {
                    CardId ="12004",
                    EffectType=typeof(NoneEffect),//效果
                    Name="利维亚的杰洛特",
                    Strength=15,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "如果要付出这种代价方能拯救世界，那就让世界毁灭算了。",
                    Info = "没有特殊技能。",
                    CardArtsId = "11210300",
                }
            },
            {
                "12005",//希里：冲刺
                new GwentCard()
                {
                    CardId ="12005",
                    EffectType=typeof(CiriDash),//效果
                    Name="希里：冲刺",
                    Strength=11,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cintra,Categorie.Witcher},//需要添加
                    Flavor = "你知道童话几时成真吗？大家都开始相信的时候。",
                    Info = "被置入墓场时返回牌组，并获得3点强化。",
                    CardArtsId = "11211000",
                }
            },
            {
                "12006",//萨琪亚萨司：龙焰
                new GwentCard()
                {
                    CardId ="12006",
                    EffectType=typeof(SaesenthessisBlaze),//效果
                    Name="萨琪亚萨司：龙焰",
                    Strength=11,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Aedirn,Categorie.Draconid},//需要添加
                    Flavor = "我继承了父亲的变身能力……好吧，尽管我只有一种变化形态。",
                    Info = "放逐所有手牌，抽同等数量的牌。",
                    CardArtsId = "20005700",
                }
            },
            {
                "12007",//特莉丝·梅莉葛德
                new GwentCard()
                {
                    CardId ="12007",
                    EffectType=typeof(TrissMerigold),//效果
                    Name="特莉丝·梅莉葛德",
                    Strength=10,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Temeria},//需要添加
                    Flavor = "我能照顾自己，相信我。",
                    Info = "造成5点伤害。",
                    CardArtsId = "11210600",
                }
            },
            {
                "12008",//维伦特瑞坦梅斯
                new GwentCard()
                {
                    CardId ="12008",
                    EffectType=typeof(Villentretenmerth),//效果
                    Name="维伦特瑞坦梅斯",
                    Strength=10,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Draconid},//需要添加
                    Flavor = "他还自称“三寒鸦”博尔奇……他不太会取名字。",
                    Info = "3回合后的回合开始时：摧毁场上除自身外所有最强的单位。 3点护甲。",
                    CardArtsId = "11210700",
                }
            },
            {
                "12009",//先祖麦酒
                new GwentCard()
                {
                    CardId ="12009",
                    EffectType=typeof(AleOfTheAncestors),//效果
                    Name="先祖麦酒",
                    Strength=10,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "富克斯家族的传奇创始人波罗斯因为酗酒丢了性命。当时他的金戒指掉进了一条小溪，他去捞的时候晕了过去。",
                    Info = "在所在排洒下“黄金酒沫”。",
                    CardArtsId = "20024400",
                }
            },
            {
                "12010",//叶奈法：咒术师
                new GwentCard()
                {
                    CardId ="12010",
                    EffectType=typeof(YenneferTheConjurer),//效果
                    Name="叶奈法：咒术师",
                    Strength=10,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Aedirn},//需要添加
                    Flavor = "一位优秀的女术士必须知道何时该召唤出冰，何时该召唤出火。",
                    Info = "回合结束时，对所有最强的敌军单位造成1点伤害。",
                    CardArtsId = "11211300",
                }
            },
            {
                "12011",//丹德里恩：虚妄荣光
                new GwentCard()
                {
                    CardId ="12011",
                    EffectType=typeof(DandelionVainglory),//效果
                    Name="丹德里恩：虚妄荣光",
                    Strength=9,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support},//需要添加
                    Flavor = "丹德里恩大师跟我说过你所有的冒险故事。比如他是如何利用歌声来助你战斗，他优势如何用琴声驯服了巨章鱼怪……",
                    Info = "己方起始牌组中每有1张“杰洛特”、“叶奈法”、“特莉丝”或“卓尔坦”牌，便获得3点增益。",
                    CardArtsId = "20177400",
                }
            },
            {
                "12012",//阿瓦拉克
                new GwentCard()
                {
                    CardId ="12012",
                    EffectType=typeof(AvallacH),//效果
                    Name="阿瓦拉克",
                    Strength=8,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Elf},//需要添加
                    Flavor = "你们人类的品味……很独特。",
                    Info = "休战：双方各抽2张牌。",
                    CardArtsId = "13210500",
                }
            },
            {
                "12013",//兰伯特：剑术大师
                new GwentCard()
                {
                    CardId ="12013",
                    EffectType=typeof(LambertSowrdmaster),//效果
                    Name="兰伯特：剑术大师",
                    Strength=8,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "不用外行人来教我！",
                    Info = "对一个敌军单位的所有同名牌造成4点伤害。",
                    CardArtsId = "20023500",
                }
            },
            {
                "12014",//特莉丝：蝴蝶咒语
                new GwentCard()
                {
                    CardId ="12014",
                    EffectType=typeof(TrissButterflySpell),//效果
                    Name="特莉丝：蝴蝶咒语",
                    Strength=8,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Temeria},//需要添加
                    Flavor = "长官……我们的箭，它们……它们长出翅膀了！",
                    Info = "回合结束时，使其他最弱的友军单位获得1点增益。",
                    CardArtsId = "12210700",
                }
            },
            {
                "12015",//卓尔坦：流氓
                new GwentCard()
                {
                    CardId ="12015",
                    EffectType=typeof(ZoltanScoundrel),//效果
                    Name="卓尔坦：流氓",
                    Strength=8,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Dwarf},//需要添加
                    Flavor = "请原谅，这只外国鸟儿聪明归聪明，就是太粗俗，可花了我十个塔勒呢。",
                    Info = "择一：生成“话篓子：伙伴”：使2个相邻单位获得2点增益；或生成“话篓子：捣蛋鬼”：对2个相邻单位造成2点伤害。",
                    CardArtsId = "11210900",
                }
            },
            {
                "12016",//艾斯卡尔：寻路者
                new GwentCard()
                {
                    CardId ="12016",
                    EffectType=typeof(EskelPathfinder),//效果
                    Name="艾斯卡尔：寻路者",
                    Strength=7,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "白狼，我只是个普通猎魔人。我不猎龙，不跟国王称兄道弟，也不和女术士纠缠……",
                    Info = "摧毁1个没有被增益的铜色/银色敌军单位。",
                    CardArtsId = "20023600",
                }
            },
            {
                "12017",//杰洛特：猎魔大师
                new GwentCard()
                {
                    CardId ="12017",
                    EffectType=typeof(GeraltProfessional),//效果
                    Name="杰洛特：猎魔大师",
                    Strength=7,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "我曾经完成过一份委托。对方要我选择奖赏，我便依照意外率向他索要回报。",
                    Info = "对1个敌军单位造成4点伤害。若它为“怪兽”单位，则直接将其摧毁。",
                    CardArtsId = "20175900",
                }
            },
            {
                "12018",//伊瓦拉夸克斯
                new GwentCard()
                {
                    CardId ="12018",
                    EffectType=typeof(Ihuarraquax),//效果
                    Name="伊瓦拉夸克斯",
                    Strength=7,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast},//需要添加
                    Flavor = "“真是难以置信，”希里回过神来想道，“在这个世界，独角兽已经不存在了。它们早就灭绝了。”",
                    Info = "对自身造成5点伤害。 当前战力等同于基础战力时，在回合结束时对3个敌方随机单位造成7点伤害。",
                    CardArtsId = "20005100",
                }
            },
            {
                "12019",//希里
                new GwentCard()
                {
                    CardId ="12019",
                    EffectType=typeof(Ciri),//效果
                    Name="希里",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cintra,Categorie.Witcher},//需要添加
                    Flavor = "去往何处，何时动身，我自己说了算。",
                    Info = "己方输掉小局时返回手牌。 2点护甲。",
                    CardArtsId = "11210100",
                }
            },
            {
                "12020",//刚特·欧迪姆
                new GwentCard()
                {
                    CardId ="12020",
                    EffectType=typeof(GaunterODimm),//效果
                    Name="刚特·欧迪姆",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Relict},//需要添加
                    Flavor = "他会一字不差地实现你的愿望，但往往问题就在于此。",
                    Info = "发牌员随机创造一张单位牌，你猜测其战力是大于、等于或小于6。如果你猜对了打出该牌。",
                    CardArtsId = "13221500",
                }
            },
            {
                "12021",//杰洛特：阿尔德法印
                new GwentCard()
                {
                    CardId ="12021",
                    EffectType=typeof(GeraltAard),//效果
                    Name="杰洛特：阿尔德法印",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "这股注入专注力的能量能摧毁一切挡道之物——如果你忘了带钥匙，这招最管用。",
                    Info = "选择3个敌军单位各造成3点伤害，并将它们上移1排。",
                    CardArtsId = "11211100",
                }
            },
            {
                "12022",//雷吉斯：高等吸血鬼
                new GwentCard()
                {
                    CardId ="12022",
                    EffectType=typeof(RegisHigherVampire),//效果
                    Name="雷吉斯：高等吸血鬼",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Vampire},//需要添加
                    Flavor = "他能随心所欲地隐身，用目光让猎物沉睡，饱餐后更能化为蝙蝠，高飞遁走。真是太厉害了。",
                    Info = "检视对方牌组3张铜色单位牌。选择1张吞噬，获得等同于其基础战力的增益。",
                    CardArtsId = "11210500",
                }
            },
            {
                "12023",//维瑟米尔：导师
                new GwentCard()
                {
                    CardId ="12023",
                    EffectType=typeof(VesemirMentor),//效果
                    Name="维瑟米尔：导师",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "讨伐怪物可不是什么儿戏。希里必须明白这一点，才能成为一名猎魔人。",
                    Info = "从牌组打出1张铜色/银色“炼金”牌。",
                    CardArtsId = "20023700",
                }
            },
            {
                "12024",//叶奈法
                new GwentCard()
                {
                    CardId ="12024",
                    EffectType=typeof(Yennefer),//效果
                    Name="叶奈法",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Aedirn},//需要添加
                    Flavor = "魔法是混沌，是艺术，也是科学。它是诅咒，是祝福，也是进步",
                    Info = "择一：生成“独角兽”：使除自身外所有单位获得2点增益；或生成“梦魇独角兽”：对除自身外所有单位造成2点伤害。",
                    CardArtsId = "11210800",
                }
            },
            {
                "12025",//杰洛特：亚登法印
                new GwentCard()
                {
                    CardId ="12025",
                    EffectType=typeof(GeraltYrden),//效果
                    Name="杰洛特：亚登法印",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "他在已经变成木乃伊的雅妲身边躺下，在她的石棺盖内测画下了亚登法印。",
                    Info = "重置单排所有单位，并移除它们的状态。",
                    CardArtsId = "20152300",
                }
            },
            {
                "12026",//特莉丝：心灵传动
                new GwentCard()
                {
                    CardId ="12026",
                    EffectType=typeof(TrissTelekinesis),//效果
                    Name="特莉丝：心灵传动",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Temeria},//需要添加
                    Flavor = "捆住手脚远远不够。塞住嘴巴也不会让她的危险程度有分毫减少。所以，阻魔金是唯一的解决方案。",
                    Info = "创造任意方起始牌组中的1张铜色特殊牌。",
                    CardArtsId = "20177300",
                }
            },
            {
                "12002",//杰洛特：伊格尼法印
                new GwentCard()
                {
                    CardId ="12002",
                    EffectType=typeof(GeraltIgni),//效果
                    Name="杰洛特：伊格尼法印",
                    Strength=5,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "猎魔人晃晃手指就能点灯，或把敌人烧成灰。",
                    Info = "若对方某排总战力不低于25点，则摧毁其上所有最强的单位。",
                    CardArtsId = "11210200",
                }
            },
            {
                "12027",//狐妖
                new GwentCard()
                {
                    CardId ="12027",
                    EffectType=typeof(Aguara),//效果
                    Name="狐妖",
                    Strength=5,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Relict},//需要添加
                    Flavor = "乖乖听话，不然就让狐妖把你抓走！",
                    Info = "择二：使最弱的友军单位获得5点增益；使手牌中的1个随机单位获得5点增益；对最强的1个敌军单位造成5点伤害；魅惑1个战力不高于5点的敌军“精灵”单位。",
                    CardArtsId = "20006200",
                }
            },
            {
                "12028",//凤凰
                new GwentCard()
                {
                    CardId ="12028",
                    EffectType=typeof(Phoenix),//效果
                    Name="凤凰",
                    Strength=5,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Draconid,Categorie.Doomed},//需要添加
                    Flavor = "是先有鸡还是先有蛋？和凤凰相比，这个问题根本不值一提。",
                    Info = "复活1个铜色/银色“龙兽”单位。",
                    CardArtsId = "20157900",
                }
            },
            {
                "12003",//丹德里恩：传奇诗人
                new GwentCard()
                {
                    CardId ="12003",
                    EffectType=typeof(DandelionPoet),//效果
                    Name="丹德里恩：传奇诗人",
                    Strength=5,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support},//需要添加
                    Flavor = "文胜于武，笔胜于剑。",
                    Info = "抽1张牌，随后打出1张牌。",
                    CardArtsId = "20177600",
                }
            },
            {
                "12029",//阿瓦拉克：贤者
                new GwentCard()
                {
                    CardId ="12029",
                    EffectType=typeof(AvallacHTheSage),//效果
                    Name="阿瓦拉克：贤者",
                    Strength=3,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Elf},//需要添加
                    Flavor = "在自由的精灵之中，有着极少数的艾恩·萨维尼，即精灵语中的“贤者”。他们十分神秘，是如同传说般的存在。",
                    Info = "随机生成1张对方起始牌组中金色/银色单位牌的原始同名牌。",
                    CardArtsId = "11211200",
                }
            },
            {
                "12030",//狐妖：真身
                new GwentCard()
                {
                    CardId ="12030",
                    EffectType=typeof(AguaraTrueForm),//效果
                    Name="狐妖：真身",
                    Strength=2,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Relict},//需要添加
                    Flavor = "听说过“化兽”吗？就相当于把狼人反过来：这是一种可以变成人形的怪物。",
                    Info = "不限阵营地创造1张铜色/银色“法术”牌。",
                    CardArtsId = "20005600",
                }
            },
            {
                "12031",//雷吉斯
                new GwentCard()
                {
                    CardId ="12031",
                    EffectType=typeof(Regis),//效果
                    Name="雷吉斯",
                    Strength=1,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Vampire},//需要添加
                    Flavor = "人类——按照他们委婉的说法——管我叫怪物和吸血的恶魔。",
                    Info = "汲食1个单位的所有增益。",
                    CardArtsId = "11210400",
                }
            },
            {
                "12032",//希里：新星
                new GwentCard()
                {
                    CardId ="12032",
                    EffectType=typeof(CiriNova),//效果
                    Name="希里：新星",
                    Strength=1,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cintra,Categorie.Witcher,Categorie.Doomed},//需要添加
                    Flavor = "吉薇艾儿无法掌控自己所拥有的非凡力量。对她自己，对其他人来说，她都是个危险。除非她学会控制这份力量，否则不能把她放出来。",
                    Info = "若每张铜色牌在己方初始牌组中刚好有2张，则基础战力变为22点。",
                    CardArtsId = "20162600",
                }
            },
            {
                "12033",//科拉兹热浪
                new GwentCard()
                {
                    CardId ="12033",
                    EffectType=typeof(KorathiHeatwave),//效果
                    Name="科拉兹热浪",
                    Strength=0,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Hazard},//需要添加
                    Flavor = "维可瓦罗学者认定，缺少了帝国的援助，饱受干旱困扰的行省会失去一半的人口，三分之二的牲畜，以及全部的反叛意志。",
                    Info = "灾厄降于对方全场。 回合开始时，对各排最弱的单位造成2点伤害。",
                    CardArtsId = "20001800",
                }
            },
            {
                "12034",//终末之战
                new GwentCard()
                {
                    CardId ="12034",
                    EffectType=typeof(RaghNarRoog),//效果
                    Name="终末之战",
                    Strength=0,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special,Categorie.Hazard},//需要添加
                    Flavor = "终焉纪元到来时，汉姆多尔将挺身而出，迎击来自摩霍格的邪恶军团——由混沌而生的妖灵、魔鬼和恶灵。",
                    Info = "灾厄降于对方全场。回合开始时，对各排最强的单位造成2点伤害。",
                    CardArtsId = "11310100",
                }
            },
            {
                "12035",//复原
                new GwentCard()
                {
                    CardId ="12035",
                    EffectType=typeof(Renew),//效果
                    Name="复原",
                    Strength=0,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special},//需要添加
                    Flavor = "医学之奇迹，魔法之神力。",
                    Info = "复活己方1个非领袖金色单位。",
                    CardArtsId = "11331600",
                }
            },
            {
                "12001",//皇家谕令
                new GwentCard()
                {
                    CardId ="12001",
                    EffectType=typeof(RoyalDecree),//效果
                    Name="皇家谕令",
                    Strength=0,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special},//需要添加
                    Flavor = "吾，弗尔泰斯特，以泰莫利亚圣君、索登亲王、布鲁格守护者及其他各种合法称号之名义，做出如下判决……",
                    Info = "从牌组打出1张金色单位牌，使其获得2点增益。",
                    CardArtsId = "20015400",
                }
            },
            {
                "12036",//嘴套
                new GwentCard()
                {
                    CardId ="12036",
                    EffectType=typeof(Muzzle),//效果
                    Name="嘴套",
                    Strength=0,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "不是所有的野兽都能被驯服。但嘴套谁都能戴。",
                    Info = "魅惑1个战力不高于8点的敌军铜色/银色单位。",
                    CardArtsId = "20022500",
                }
            },
            {
                "12037",//附子草
                new GwentCard()
                {
                    CardId ="12037",
                    EffectType=typeof(Wolfsbane),//效果
                    Name="附子草",
                    Strength=0,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Organic},//需要添加
                    Flavor = "附子草又名“毒药女王”，常见于多种猎魔人药水和炼金药剂。",
                    Info = "在墓场停留3个回合后，在回合结束时，对最强的敌军单位造成6点伤害，使最弱的友军单位获得6点增益。",
                    CardArtsId = "20022600",
                }
            },
            {
                "12038",//哈马维的梦境
                new GwentCard()
                {
                    CardId ="12038",
                    EffectType=typeof(HanmarvynSDream),//效果
                    Name="哈马维的梦境",
                    Strength=0,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special},//需要添加
                    Flavor = "这道法术可以让你见到死人生前的最后一刻……前提是你得在施法的时候活下来。",
                    Info = "生成对方墓场中1张非领袖金色单位牌的原始同名，并使其获得2点增益。",
                    CardArtsId = "20007900",
                }
            },
            {
                "12039",//乌马的诅咒
                new GwentCard()
                {
                    CardId ="12039",
                    EffectType=typeof(UmaSCurese),//效果
                    Name="乌马的诅咒",
                    Strength=0,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special},//需要添加
                    Flavor = "我给了你三条确凿的线索，脉络比晨露还要清楚，就连我手下的探子和宫廷女术士都拨给你了。可你没能找到我的女儿，反而带来了这头……怪物？",
                    Info = "不限阵营地创造1个非领袖金色单位。",
                    CardArtsId = "20005800",
                }
            },
            {
                "12040",//青草试炼
                new GwentCard()
                {
                    CardId ="12040",
                    EffectType=typeof(TrialOfTheGrasses),//效果
                    Name="青草试炼",
                    Strength=0,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special},//需要添加
                    Flavor = "想象有一团黏土。为了塑形，你首先必须把它打湿，不然就会开裂。试炼的第一步就是这样。可以说，它会开放肉体，只有这样，突变诱发物才能将其塑造成猎魔人。",
                    Info = "使1个“猎魔人”单位增益至25点战力；或对1个非“猎魔人”单位造成10点伤害，若目标存活，则使其增益至25点战力。",
                    CardArtsId = "20007800",
                }
            },
            {
                "12041",//店店的大冒险
                new GwentCard()
                {
                    CardId ="12041",
                    EffectType=typeof(ShupeSDayOff),//效果
                    Name="店店的大冒险",
                    Strength=0,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Organic},//需要添加
                    Flavor = "其他巨魔都觉得他是个异类，毕竟在巨魔们看来，谁会喜欢彩色纸片胜过喜欢石头呢？",
                    Info = "若己方起始牌组没有重复牌，则派“店店”去冒险。",
                    CardArtsId = "20027500",
                }
            },
            {
                "12042",//矮人符文剑
                new GwentCard()
                {
                    CardId ="12042",
                    EffectType=typeof(Sihil),//效果
                    Name="矮人符文剑",
                    Strength=0,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "“这把剑上刻的是什么？诅咒吗？”“不，是脏话。”",
                    Info = "择一：对所有战力为“奇数”的敌军单位造成3点伤害；对所有战力为“偶数”的敌军单位造成3点伤害；或从牌组随机打出1个铜色/银色单位。",
                    CardArtsId = "20163200",
                }
            },
            {
                "13003",//莎拉
                new GwentCard()
                {
                    CardId ="13003",
                    EffectType=typeof(Sarah),//效果
                    Name="莎拉",
                    Strength=11,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Relict},//需要添加
                    Flavor = "来陪小莎拉玩游戏吧！",
                    Info = "交换1张颜色相同的牌。",
                    CardArtsId = "11221200",
                }
            },
            {
                "13004",//爱丽丝的同伴
                new GwentCard()
                {
                    CardId ="13004",
                    EffectType=typeof(IrisCompanions),//效果
                    Name="爱丽丝的同伴",
                    Strength=11,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Relict},//需要添加
                    Flavor = "我们的名字还是不说为好。就当我们是……主人家的朋友吧。",
                    Info = "将1张牌从牌组移至手牌，然后随机丢弃1张牌。",
                    CardArtsId = "20008300",
                }
            },
            {
                "13005",//吉尔曼
                new GwentCard()
                {
                    CardId ="13005",
                    EffectType=typeof(GermainPiquant),//效果
                    Name="吉尔曼",
                    Strength=10,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer},//需要添加
                    Flavor = "陶森特需要这位英雄，但它不配。",
                    Info = "在两侧各生成2头“牛”。",
                    CardArtsId = "20129000",
                }
            },
            {
                "13006",//纳威伦
                new GwentCard()
                {
                    CardId ="13006",
                    EffectType=typeof(Nivellen),//效果
                    Name="纳威伦",
                    Strength=10,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed},//需要添加
                    Flavor = "迷路了？要迷路到其它地方去，只要别在我这儿瞎逛就行。把你的左耳对准太阳，一直往前，没多久就能走上大路。怎么？你还在等什么？",
                    Info = "将单排上的所有单位移至随机排。",
                    CardArtsId = "20008900",
                }
            },
            {
                "13007",//斯崔葛布
                new GwentCard()
                {
                    CardId ="13007",
                    EffectType=typeof(Stregobor),//效果
                    Name="斯崔葛布",
                    Strength=10,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage},//需要添加
                    Flavor = "猎魔人见过看上去像议员的贼，见过看上去像乞丐的议员，也见过看上去像贼的国王。不过斯崔葛布的样子，就和大众心目中法师的形象没什么两样。",
                    Info = "休战：双方各抽1张单位牌，将其战力设为1。",
                    CardArtsId = "20009100",
                }
            },
            {
                "13008",//乔尼
                new GwentCard()
                {
                    CardId ="13008",
                    EffectType=typeof(Johnny),//效果
                    Name="乔尼",
                    Strength=9,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Relict},//需要添加
                    Flavor = "要是再也没办法亲口说出“狮子头上长虱子”，生活就真的太无趣啦。",
                    Info = "丢弃1张手牌，并在手牌中添加1张对方起始牌组中颜色相同的原始同名牌。",
                    CardArtsId = "11221100",
                }
            },
            {
                "13009",//赛浦利安·威利
                new GwentCard()
                {
                    CardId ="13009",
                    EffectType=typeof(CyprianWiley),//效果
                    Name="赛浦利安·威利",
                    Strength=8,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Redania},//需要添加
                    Flavor = "诺维格瑞的黑帮四巨头之一——另外三个是西吉·卢文、卡罗·“砍刀”·凡瑞西和乞丐王。",
                    Info = "对1个单位造成4点削弱。",
                    CardArtsId = "11221400",
                }
            },
            {
                "13010",//奥克维斯塔
                new GwentCard()
                {
                    CardId ="13010",
                    EffectType=typeof(Ocvist),//效果
                    Name="奥克维斯塔",
                    Strength=8,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Draconid},//需要添加
                    Flavor = "他是石英山之主，毁灭者，图拉真的屠夫。但在闲暇时间里，他喜欢远足和烛光晚餐。",
                    Info = "力竭。 4回合后的回合开始时：对所有敌军单位造成1点伤害，随后返回手牌。",
                    CardArtsId = "11220600",
                }
            },
            {
                "13011",//卡罗“砍刀”凡瑞西
                new GwentCard()
                {
                    CardId ="13011",
                    EffectType=typeof(Cleaver),//效果
                    Name="卡罗“砍刀”凡瑞西",
                    Strength=7,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Dwarf},//需要添加
                    Flavor = "每个想要在诺维格瑞做生意的都很清楚——要么同意卡罗的条件，要么就夹着尾巴滚出去。",
                    Info = "造成等同于手牌数量的伤害。",
                    CardArtsId = "12221600",
                }
            },
            {
                "13012",//米尔加塔布雷克
                new GwentCard()
                {
                    CardId ="13012",
                    EffectType=typeof(Myrgtabrakke),//效果
                    Name="米尔加塔布雷克",
                    Strength=7,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Draconid},//需要添加
                    Flavor = "永远别想分开母龙和她的孩子。",
                    Info = "造成2点伤害，再重复2次。",
                    CardArtsId = "11220500",
                }
            },
            {
                "13013",//维瑟米尔
                new GwentCard()
                {
                    CardId ="13013",
                    EffectType=typeof(Vesemir),//效果
                    Name="维瑟米尔",
                    Strength=7,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "就算上了绞架也别放弃——让他们给你拿点水，毕竟没人知道水拿来前会发生什么。",
                    Info = "召唤“艾斯卡尔”和“兰伯特”。",
                    CardArtsId = "11220300",
                }
            },
            {
                "13014",//艾斯卡尔
                new GwentCard()
                {
                    CardId ="13014",
                    EffectType=typeof(Eskel),//效果
                    Name="艾斯卡尔",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "白狼，我只是个普通猎魔人。我不猎龙，不跟国王称兄道弟，也不和女术士纠缠……",
                    Info = "召唤“维瑟米尔”和“兰伯特”。",
                    CardArtsId = "11220400",
                }
            },
            {
                "13015",//欧吉尔德·伊佛瑞克
                new GwentCard()
                {
                    CardId ="13015",
                    EffectType=typeof(OlgierdVonEverec),//效果
                    Name="欧吉尔德·伊佛瑞克",
                    Strength=5,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Redania,Categorie.Cursed},//需要添加
                    Flavor = "至少你知道我的头不好砍了。",
                    Info = "遗愿：复活至原位。",
                    CardArtsId = "11220700",
                }
            },
            {
                "13002",//乞丐王
                new GwentCard()
                {
                    CardId ="13002",
                    EffectType=typeof(KingOfBeggars),//效果
                    Name="乞丐王",
                    Strength=5,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support},//需要添加
                    Flavor = "要是我缺鼻子或者断手了，那显然，乞丐王接受这两种付款方式。",
                    Info = "如果落后，则获得强化，直至战力持平或最多到15点。",
                    CardArtsId = "11221300",
                }
            },
            {
                "13016",//操作者
                new GwentCard()
                {
                    CardId ="13016",
                    EffectType=typeof(Operator),//效果
                    Name="操作者",
                    Strength=5,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage},//需要添加
                    Flavor = "时空在我们面前瓦解，也在我们身后膨胀，这就是穿越。",
                    Info = "力竭。 休战：为双方各添加1张己方手牌1张铜色单位牌的原始同名牌。",
                    CardArtsId = "11220800",
                }
            },
            {
                "13017",//兰伯特
                new GwentCard()
                {
                    CardId ="13017",
                    EffectType=typeof(Lambert),//效果
                    Name="兰伯特",
                    Strength=5,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "这样的沟通方式才对路嘛！",
                    Info = "召唤“维瑟米尔”和“艾斯卡尔”。",
                    CardArtsId = "11220200",
                }
            },
            {
                "13018",//德鲁伊控天者
                new GwentCard()
                {
                    CardId ="13018",
                    EffectType=typeof(Vaedermakar),//效果
                    Name="德鲁伊控天者",
                    Strength=4,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage},//需要添加
                    Flavor = "控天者德鲁伊能操控各种元素之力，让狂风暴雨化为绕指柔风，降下毁天灭地的雹暴，还能拖雷掣电让敌军灰飞烟灭……所以我给你个忠告：面对他，一定要毕恭毕敬。",
                    Info = "生成“刺骨冰霜”、“蔽日浓雾”或“阿尔祖落雷术”。",
                    CardArtsId = "11320800",
                }
            },
            {
                "13001",//萝卜
                new GwentCard()
                {
                    CardId ="13001",
                    EffectType=typeof(Roach),//效果
                    Name="萝卜",
                    Strength=4,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast},//需要添加
                    Flavor = "杰洛特，我们得来场人马间的对话。恕我直言，你的骑术……真的有待提高，伙计。",
                    Info = "己方从手牌打出金色单位牌时，召唤此单位。",
                    CardArtsId = "11221000",
                }
            },
            {
                "13019",//爱丽丝·伊佛瑞克
                new GwentCard()
                {
                    CardId ="13019",
                    EffectType=typeof(IrisVonEverec),//效果
                    Name="爱丽丝·伊佛瑞克",
                    Strength=3,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Redania,Categorie.Cursed},//需要添加
                    Flavor = "我的回忆所剩无几……但每次想到我的玫瑰，记忆便会涌现。",
                    Info = "间谍。 遗愿：使对面半场5个随机单位获得5点增益。",
                    CardArtsId = "11221500",
                }
            },
            {
                "13020",//多瑞加雷
                new GwentCard()
                {
                    CardId ="13020",
                    EffectType=typeof(DorregarayOfVole),//效果
                    Name="多瑞加雷",
                    Strength=1,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage},//需要添加
                    Flavor = "和猎魔人一样，多瑞加雷也热爱同怪物打交道。不过他有自己的一套分类系统。别人眼里面目可憎的食尸生物、食人魔，在他看来都特别可爱。",
                    Info = "不限阵营地创造1个铜色/银色“龙兽”或“野兽”单位。",
                    CardArtsId = "20008700",
                }
            },
            {
                "13021",//杜度
                new GwentCard()
                {
                    CardId ="13021",
                    EffectType=typeof(Dudu),//效果
                    Name="杜度",
                    Strength=1,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Relict},//需要添加
                    Flavor = "拟态怪有很多别名：易形怪、二重身、模仿怪……变形怪。",
                    Info = "复制一个敌军单位的战力。",
                    CardArtsId = "11220100",
                }
            },
            {
                "13022",//获奖奶牛
                new GwentCard()
                {
                    CardId ="13022",
                    EffectType=typeof(PrizeWinningCow),//效果
                    Name="获奖奶牛",
                    Strength=1,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast},//需要添加
                    Flavor = "哞～～～",
                    Info = "遗愿：在同排生成1个“羊角魔”。",
                    CardArtsId = "11220900",
                }
            },
            {
                "13023",//黑血
                new GwentCard()
                {
                    CardId ="13023",
                    EffectType=typeof(BlackBlood),//效果
                    Name="黑血",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "吸血鬼们纷纷表示：使用这种药水有违体育精神。",
                    Info = "择一：创造1个铜色“食腐生物”或“吸血鬼”单位，并使其获得2点增益；或摧毁1个铜色/银色“食腐生物”或“吸血鬼”单位。",
                    CardArtsId = "20169700",
                }
            },
            {
                "13024",//阿尔祖召唤术
                new GwentCard()
                {
                    CardId ="13024",
                    EffectType=typeof(AlzurSDoubleCross),//效果
                    Name="阿尔祖召唤术",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special},//需要添加
                    Flavor = "阿尔祖创造的一些怪物仍在四处游荡，其中便有令人胆寒的巨蜈蚣——它杀掉了创造自己的法师，摧毁了半个马里波，然后逃进了河谷地区幽暗的森林。",
                    Info = "使牌组中最强的铜色/银色单位牌获得2点增益，然后打出它。",
                    CardArtsId = "11320900",
                }
            },
            {
                "13025",//化器封形
                new GwentCard()
                {
                    CardId ="13025",
                    EffectType=typeof(ArtefactCompression),//效果
                    Name="化器封形",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special},//需要添加
                    Flavor = "雕像瞬间爆开，颤动不已，犹如一道在地上爬行的烟雾，变换着自己的形状。道道光芒里，有东西上下纷飞，不断成形。片刻之后，魔法圈的正中间突然现出了一道人影。",
                    Info = "将1个铜色/银色单位变为“翡翠人偶”。",
                    CardArtsId = "20005300",
                }
            },
            {
                "13026",//贝克尔的黑暗之镜
                new GwentCard()
                {
                    CardId ="13026",
                    EffectType=typeof(BekkerSDarkMirror),//效果
                    Name="贝克尔的黑暗之镜",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special},//需要添加
                    Flavor = "当你凝视深渊的时候，深渊也在凝视着你。",
                    Info = "对场上最强的单位造成最多10点伤害（无视护甲），并使场上最弱的单位获得相同数值的增益。",
                    CardArtsId = "11331500",
                }
            },
            {
                "13027",//指挥号角
                new GwentCard()
                {
                    CardId ="13027",
                    EffectType=typeof(CommanderSHorn),//效果
                    Name="指挥号角",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special},//需要添加
                    Flavor = "士气加一分，听力减三分。",
                    Info = "使5个相邻单位获得3点增益。",
                    CardArtsId = "11320700",
                }
            },
            {
                "13028",//诱饵
                new GwentCard()
                {
                    CardId ="13028",
                    EffectType=typeof(Decoy),//效果
                    Name="诱饵",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special},//需要添加
                    Flavor = "如果拿来应急，假人也是不错的挡箭牌。",
                    Info = "重新打出1个铜色/银色友军单位，并使它获得3点增益。",
                    CardArtsId = "11320100",
                }
            },
            {
                "13029",//阻魔金炸弹
                new GwentCard()
                {
                    CardId ="13029",
                    EffectType=typeof(DimeritiumBomb),//效果
                    Name="阻魔金炸弹",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special},//需要添加
                    Flavor = "女巫猎人必备。无声的闪光过后，最强大的法师也得乖乖就擒。",
                    Info = "重置单排上所有的受增益单位。",
                    CardArtsId = "11320500",
                }
            },
            {
                "13030",//蝎尾狮毒液
                new GwentCard()
                {
                    CardId ="13030",
                    EffectType=typeof(ManticoreVenom),//效果
                    Name="蝎尾狮毒液",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Organic},//需要添加
                    Flavor = "剧毒致命的速度快得让你连尼弗迦德皇帝的头衔都念不完。",
                    Info = "造成13点伤害。",
                    CardArtsId = "11330600",
                }
            },
            {
                "13031",//行军令
                new GwentCard()
                {
                    CardId ="13031",
                    EffectType=typeof(MarchingOrders),//效果
                    Name="行军令",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special},//需要添加
                    Flavor = "我们只不过是老头子们的棋子，为他们腐朽的妄想命丧沙场……",
                    Info = "使牌组中最弱的铜色/银色单位牌获得2点增益，然后打出它。",
                    CardArtsId = "20001900",
                }
            },
            {
                "13032",//特莉丝雹暴术
                new GwentCard()
                {
                    CardId ="13032",
                    EffectType=typeof(MerigoldSHailstorm),//效果
                    Name="特莉丝雹暴术",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special},//需要添加
                    Flavor = "天空突然暗了下来，云层笼罩在城镇上空。愁云惨淡之中，寒风呼啸而过。“哦，我的天哪，”叶奈法吸了口气，“看起来你成功了……”",
                    Info = "使单排所有铜色和银色单位的战力减半。",
                    CardArtsId = "11320200",
                }
            },
            {
                "13033",//死灵术
                new GwentCard()
                {
                    CardId ="13033",
                    EffectType=typeof(Necromancy),//效果
                    Name="死灵术",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special},//需要添加
                    Flavor = "无论怎样……我们都有办法叫你开口。",
                    Info = "从双方墓场放逐1个铜色/银色单位，其战力将成为1个友军单位的增益。",
                    CardArtsId = "20002000",
                }
            },
            {
                "13034",//烧灼
                new GwentCard()
                {
                    CardId ="13034",
                    EffectType=typeof(Scorch),//效果
                    Name="烧灼",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special},//需要添加
                    Flavor = "杰洛特退了一步。他见过不少被烧弹击中的人，更准确地说，他见过不少烧弹留下的残骸。",
                    Info = "摧毁所有最强的单位。",
                    CardArtsId = "11330900",
                }
            },
            {
                "13035",//史凯利格风暴
                new GwentCard()
                {
                    CardId ="13035",
                    EffectType=typeof(SkelligeStorm),//效果
                    Name="史凯利格风暴",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Hazard},//需要添加
                    Flavor = "这可不是普通风暴，这是天神之怒。",
                    Info = "在对方单排降下灾厄。回合开始时，对所在排最左侧的单位各造成2、1、1点伤害。",
                    CardArtsId = "11320300",
                }
            },
            {
                "13036",//召唤法阵
                new GwentCard()
                {
                    CardId ="13036",
                    EffectType=typeof(SummoningCircle),//效果
                    Name="召唤法阵",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special},//需要添加
                    Flavor = "在我们所处的位面之外，还存在着许许多多的位面……只要掌握了正确的知识，就可以接触并召唤远超人类想象的生物。",
                    Info = "生成1张上张被打出的铜色/银色非“密探”单位牌的原始同名牌。",
                    CardArtsId = "20002200",
                }
            },
            {
                "13037",//最后的愿望
                new GwentCard()
                {
                    CardId ="13037",
                    EffectType=typeof(TheLastWish),//效果
                    Name="最后的愿望",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special},//需要添加
                    Flavor = "亲爱的先生们，一只气灵三个愿望，随后它们就会跑回自己的界域去。",
                    Info = "随机检视牌组的2张牌，打出1张。",
                    CardArtsId = "11310200",
                }
            },
            {
                "13038",//白霜
                new GwentCard()
                {
                    CardId ="13038",
                    EffectType=typeof(WhiteFrost),//效果
                    Name="白霜",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Hazard},//需要添加
                    Flavor = "见证“泰德戴尔瑞”——终焉纪元——这被白霜摧毀的世界吧！",
                    Info = "在对方相邻两排降下灾厄。回合开始时，对所在排最弱的单位造成2点伤害。",
                    CardArtsId = "11320600",
                }
            },
            {
                "13039",//过期的麦酒
                new GwentCard()
                {
                    CardId ="13039",
                    EffectType=typeof(ExpiredAle),//效果
                    Name="过期的麦酒",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special},//需要添加
                    Flavor = "这酒原本就是绿色的吗……？",
                    Info = "对敌军每排最强的单位造成6点伤害。",
                    CardArtsId = "20023200",
                }
            },
            {
                "13040",//曼德拉草
                new GwentCard()
                {
                    CardId ="13040",
                    EffectType=typeof(Mandrake),//效果
                    Name="曼德拉草",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Organic},//需要添加
                    Flavor = "衮衮诸公，瞠目环立，不肯相信这高贵的发现。有的在谈曼陀罗花，有的又说是在用黑犬。",
                    Info = "择一：治愈1个单位，使其获得6点强化；或重置1个单位，使其受到6点削弱。",
                    CardArtsId = "20022300",
                }
            },
            {
                "13041",//贝克尔的岩崩术
                new GwentCard()
                {
                    CardId ="13041",
                    EffectType=typeof(BekkerSRockslide),//效果
                    Name="贝克尔的岩崩术",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special},//需要添加
                    Flavor = "只要一块小石子，我们就完蛋了。",
                    Info = "对最多10个随机敌军单位造成2点伤害。",
                    CardArtsId = "20163400",
                }
            },
            {
                "13042",//豪猎而归
                new GwentCard()
                {
                    CardId ="13042",
                    EffectType=typeof(GloriousHunt),//效果
                    Name="豪猎而归",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special},//需要添加
                    Flavor = "以诸神的名义，猎魔人，你把这鬼东西拿来干嘛？！“我要那畜生的脑袋！”这句话不过是打个比方！",
                    Info = "如果落后，生成1只“帝国蝎尾狮”；如果领先，生成“蝎尾狮毒液”。",
                    CardArtsId = "20153200",
                }
            },
            {
                "13043",//龙之梦
                new GwentCard()
                {
                    CardId ="13043",
                    EffectType=typeof(DragonSDream),//效果
                    Name="龙之梦",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "在瑟瑞卡尼亚，龙神崇拜体现在日常生活的各个方面。因此也难怪他们会以此为武器命名了。",
                    Info = "在对方单排降下灾厄，当任一玩家打出一张非同名“特殊”牌时，对该排上所有单位造成4点伤害。",
                    CardArtsId = "20154800",
                }
            },
            {
                "13044",//军营
                new GwentCard()
                {
                    CardId ="13044",
                    EffectType=typeof(Garrison),//效果
                    Name="军营",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special},//需要添加
                    Flavor = "咚咚咚，有人在家吗？",
                    Info = "创造对方起始牌组中的1张铜色/银色单位牌，并使它获得2点增益。",
                    CardArtsId = "20055500",
                }
            },
            {
                "14002",//战意激升
                new GwentCard()
                {
                    CardId ="14002",
                    EffectType=typeof(AdrenalineRush),//效果
                    Name="战意激升",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Organic},//需要添加
                    Flavor = "那野兽狂暴地扑来，眼中怒火熊熊，全然不顾疼痛和抵抗者的拼命反击。挡它者，唯有一死……",
                    Info = "改变1个单位的坚韧状态。",
                    CardArtsId = "11330700",
                }
            },
            {
                "14003",//阿尔祖落雷术
                new GwentCard()
                {
                    CardId ="14003",
                    EffectType=typeof(AlzurSThunder),//效果
                    Name="阿尔祖落雷术",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special},//需要添加
                    Flavor = "我们全然无法念诵“阿尔祖落雷术”这样深奥复杂的咒语。据说，阿尔祖声如狩猎号角，言若讲演名家。",
                    Info = "造成9点伤害。",
                    CardArtsId = "11330100",
                }
            },
            {
                "14004",//蟹蜘蛛毒液
                new GwentCard()
                {
                    CardId ="14004",
                    EffectType=typeof(ArachasVenom),//效果
                    Name="蟹蜘蛛毒液",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Organic},//需要添加
                    Flavor = "“若不慎接触眼部，请立即用冷水冲洗，然后起草遗嘱。”",
                    Info = "对3个相邻单位造成4点伤害。",
                    CardArtsId = "6010200",
                }
            },
            {
                "14005",//刺骨冰霜
                new GwentCard()
                {
                    CardId ="14005",
                    EffectType=typeof(BitingFrost),//效果
                    Name="刺骨冰霜",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Hazard},//需要添加
                    Flavor = "寒霜凛冽的好处就是尸体不会腐烂得那么快。",
                    Info = "在对方单排降下灾厄。回合开始时，对所在排最弱的单位造成2点伤害。",
                    CardArtsId = "11330200",
                }
            },
            {
                "14006",//惊悚咆哮
                new GwentCard()
                {
                    CardId ="14006",
                    EffectType=typeof(BloodcurdlingRoar),//效果
                    Name="惊悚咆哮",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Organic},//需要添加
                    Flavor = "起初我们碰上了一头熊……悲剧就从那时开始了。",
                    Info = "摧毁1个友军单位。 生成1头“熊”。",
                    CardArtsId = "15240600",
                }
            },
            {
                "14007",//阻魔金镣铐
                new GwentCard()
                {
                    CardId ="14007",
                    EffectType=typeof(DimeritiumShackles),//效果
                    Name="阻魔金镣铐",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "瑞达尼亚人将巫师的手腕拧到背后，给他戴上镣铐，并使劲晃了晃。特拉诺瓦叫喊挣扎，还弯下腰呕吐呻吟——杰洛特这才明白手铐的材质。",
                    Info = "改变1个单位的锁定状态。若为敌军单位，则对它造成4点伤害。",
                    CardArtsId = "11331900",
                }
            },
            {
                "14008",//瘟疫
                new GwentCard()
                {
                    CardId ="14008",
                    EffectType=typeof(Epidemic),//效果
                    Name="瘟疫",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special,Categorie.Organic},//需要添加
                    Flavor = "瘟疫不仁，以万物为刍狗。",
                    Info = "摧毁所有最弱的单位。",
                    CardArtsId = "11330800",
                }
            },
            {
                "14009",//破晓
                new GwentCard()
                {
                    CardId ="14009",
                    EffectType=typeof(FirstLight),//效果
                    Name="破晓",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special},//需要添加
                    Flavor = "太阳出来了，德洛米！太阳出来了！也许我们命不该绝……",
                    Info = "择一：使灾厄下的所有受伤友军单位获得2点增益，并清除己方半场所有灾厄；或从牌组随机打出1张铜色单位牌。",
                    CardArtsId = "11330300",
                }
            },
            {
                "14010",//佩特里的魔药
                new GwentCard()
                {
                    CardId ="14010",
                    EffectType=typeof(PetriSPhilter),//效果
                    Name="佩特里的魔药",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "但在那一夜，月色如血。",
                    Info = "使最多6个友军随机单位获得2点增益。",
                    CardArtsId = "11332100",
                }
            },
            {
                "14011",//蔽日浓雾
                new GwentCard()
                {
                    CardId ="14011",
                    EffectType=typeof(ImpenetrableFog),//效果
                    Name="蔽日浓雾",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Hazard},//需要添加
                    Flavor = "优秀指挥官的福音……拙劣指挥官的噩梦。",
                    Info = "在对方单排降下灾厄。回合开始时，对所在排最强的单位造成2点伤害。",
                    CardArtsId = "11330500",
                }
            },
            {
                "14012",//撕裂
                new GwentCard()
                {
                    CardId ="14012",
                    EffectType=typeof(Lacerate),//效果
                    Name="撕裂",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Organic},//需要添加
                    Flavor = "那绝对是你前所未见的恐怖场景——可怜的家伙……躺倒在地，任凭怪兽肆意摆布。",
                    Info = "对单排所有单位造成3点伤害。",
                    CardArtsId = "15330100",
                }
            },
            {
                "14013",//致幻菌菇
                new GwentCard()
                {
                    CardId ="14013",
                    EffectType=typeof(Mardroeme),//效果
                    Name="致幻菌菇",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Organic},//需要添加
                    Flavor = "吃夠量，世界就会变个样……",
                    Info = "择一：重置1个单位，并使其获得3点强化；或重置1个单位，使其受到3点削弱。",
                    CardArtsId = "11340300",
                }
            },
            {
                "14014",//复仇
                new GwentCard()
                {
                    CardId ="14014",
                    EffectType=typeof(Shrike),//效果
                    Name="复仇",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "虽对人类致命，但药水的毒性对猎魔人来说却微乎其微。",
                    Info = "对最多6个敌军随机单位造成2点伤害。",
                    CardArtsId = "20000900",
                }
            },
            {
                "14015",//翼手龙鳞甲盾牌
                new GwentCard()
                {
                    CardId ="14015",
                    EffectType=typeof(WyvernScaleShield),//效果
                    Name="翼手龙鳞甲盾牌",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "比一般的盾牌要坚固，而且更时髦。",
                    Info = "使1个单位获得等同于手牌中1张铜色/银色单位牌基础战力的增益。",
                    CardArtsId = "20154200",
                }
            },
            {
                "14016",//斯丹莫福德的山崩术
                new GwentCard()
                {
                    CardId ="14016",
                    EffectType=typeof(StammelfordSTremor),//效果
                    Name="斯丹莫福德的山崩术",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special},//需要添加
                    Flavor = "巫师斯丹莫福德曾将一座挡住他高塔视线的大山移走。传言他有这等移山之力，全因得到了一只地灵——也就是土界灵——的服务。",
                    Info = "对所有敌军单位造成1点伤害。",
                    CardArtsId = "11320400",
                }
            },
            {
                "14017",//燕子
                new GwentCard()
                {
                    CardId ="14017",
                    EffectType=typeof(Swallow),//效果
                    Name="燕子",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "这种药水能加速伤口的结痂和痊愈，而它之所以叫做“燕子”，因为雨燕是春天与希望的象征。",
                    Info = "使1个单位获得10点增益。",
                    CardArtsId = "11331000",
                }
            },
            {
                "14018",//雷霆
                new GwentCard()
                {
                    CardId ="14018",
                    EffectType=typeof(Thunderbolt),//效果
                    Name="雷霆",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "猎魔人脸色陡变……平日的风度荡然无存，令人毛骨悚然。",
                    Info = "使3个相邻单位获得3点增益和2点护甲。",
                    CardArtsId = "11331100",
                }
            },
            {
                "14019",//倾盆大雨
                new GwentCard()
                {
                    CardId ="14019",
                    EffectType=typeof(TorrentialRain),//效果
                    Name="倾盆大雨",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Hazard},//需要添加
                    Flavor = "这儿连雨都带股尿骚味。",
                    Info = "在对方单排降下灾厄。回合开始时，对所在排最多2个随机单位造成1点伤害。",
                    CardArtsId = "11331200",
                }
            },
            {
                "14020",//玛哈坎麦酒
                new GwentCard()
                {
                    CardId ="14020",
                    EffectType=typeof(MahakamAle),//效果
                    Name="玛哈坎麦酒",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special},//需要添加
                    Flavor = "无可争议是矮人们对世界文化所作出的最杰出贡献。",
                    Info = "使己方每排的1个随机单位获得4点增益。",
                    CardArtsId = "20044800",
                }
            },
            {
                "14021",//乌鸦眼
                new GwentCard()
                {
                    CardId ="14021",
                    EffectType=typeof(CrowSEye),//效果
                    Name="乌鸦眼",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Organic},//需要添加
                    Flavor = "某个大名鼎鼎的海盗曾经沉迷于这种迷人的药草，进而得名“乌鸦眼”。然而对于海盗故事十分讲究的人而言，他的这一段传说实在太不像话，自然也就没能流传下来。",
                    Info = "对对方每排最强的敌军单位造成4点伤害。己方墓场每有1张同名牌，便使伤害提高1点。",
                    CardArtsId = "20022400",
                }
            },
            {
                "14022",//变形怪
                new GwentCard()
                {
                    CardId ="14022",
                    EffectType=typeof(Doppler),//效果
                    Name="变形怪",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special},//需要添加
                    Flavor = "人类对变形怪深恶痛绝，觉得光是处刑还不够。因此一旦落入人类手中，他们自然就凶多吉少了……",
                    Info = "随机生成 1 张己方阵营中的铜色单位牌。",
                    CardArtsId = "20163100",
                }
            },
            {
                "14023",//乱石飞舞
                new GwentCard()
                {
                    CardId ="14023",
                    EffectType=typeof(RockBarrage),//效果
                    Name="乱石飞舞",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special},//需要添加
                    Flavor = "有一天等你老了，也会难逃被石头砸中的厄运。",
                    Info = "对1个敌军单位造成7点伤害，并将其上移一排。若该排已满，则将其摧毁。",
                    CardArtsId = "20164700",
                }
            },
            {
                "14024",//精良的长矛
                new GwentCard()
                {
                    CardId ="14024",
                    EffectType=typeof(MastercraftedSpear),//效果
                    Name="精良的长矛",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "把长矛立起来，懒虫！一匹马都不能放过去！",
                    Info = "造成等同于己方手牌中1个铜色/银色单位的基础战力的伤害。",
                    CardArtsId = "20150200",
                }
            },
            {
                "14001",//侦察
                new GwentCard()
                {
                    CardId ="14001",
                    EffectType=typeof(Reconnaissance),//效果
                    Name="侦察",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special},//需要添加
                    Flavor = "如果斥候没有回来，我们就掉头。乡下的人说这些树林里全是松鼠。我指的可不是啃松果的那种。",
                    Info = "检视己方牌组中2张铜色单位牌，随后打出1张。",
                    CardArtsId = "11340200",
                }
            },
            {
                "14025",//致命菌菇
                new GwentCard()
                {
                    CardId ="14025",
                    EffectType=typeof(Spores),//效果
                    Name="致命菌菇",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Organic},//需要添加
                    Flavor = "吃够量，世界就会变个样……",
                    Info = "对单排所有单位造成2点伤害，并清除其上的恩泽。",
                    CardArtsId = "11340400",
                }
            },
            {
                "14026",//黄金酒沫
                new GwentCard()
                {
                    CardId ="14026",
                    EffectType=typeof(GoldenFroth),//效果
                    Name="黄金酒沫",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Boon},//需要添加
                    Flavor = "闻到了吗？那是幸福的味道。",
                    Info = "在己方单排洒下恩泽。回合开始时，使2个随机单位获得1点增益。",
                    CardArtsId = "20174900",
                }
            },
            {
                "14027",//农民民兵
                new GwentCard()
                {
                    CardId ="14027",
                    EffectType=typeof(PeasantMilitia),//效果
                    Name="农民民兵",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special},//需要添加
                    Flavor = "瞧，我们是民兵。我们保卫和平",
                    Info = "在己方单排生成3个“农民”单位。",
                    CardArtsId = "20167700",
                }
            },
            {
                "15001",//店店：骑士
                new GwentCard()
                {
                    CardId ="15001",
                    EffectType=typeof(ShupeKnight),//效果
                    Name="店店：骑士",
                    Strength=12,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Ogroid,Categorie.Token},//需要添加
                    Flavor = "其他巨魔都觉得他是个异类，毕竟在巨魔们看来，谁会喜欢彩色纸片胜过喜欢石头呢？",
                    Info = "派“店店”去帝国宫廷军事学院。 强化自身至25点；坚韧；与1个敌军单位对决；重置1个单位；摧毁所有战力低于4点的敌军单位。",
                    CardArtsId = "20173700",
                }
            },
            {
                "15002",//店店：猎人
                new GwentCard()
                {
                    CardId ="15002",
                    EffectType=typeof(ShupeHunter),//效果
                    Name="店店：猎人",
                    Strength=8,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Ogroid,Categorie.Token},//需要添加
                    Flavor = "其他巨魔都觉得他是个异类，毕竟在巨魔们看来，谁会喜欢彩色纸片胜过喜欢石头呢？",
                    Info = "派“店店”去多尔·布雷坦纳的森林。 造成15点伤害；对一个敌军随机单位造成2点伤害，连续8次；重新打出1个铜色/银色单位，并使它获得5点增益；从牌组打出1张铜色/银色单位牌；移除己方半场的所有“灾厄”效果，并使友军单位获得1点增益。",
                    CardArtsId = "20173100",
                }
            },
            {
                "15003",//店店：法师
                new GwentCard()
                {
                    CardId ="15003",
                    EffectType=typeof(ShupeMage),//效果
                    Name="店店：法师",
                    Strength=4,
                    Group=Group.Gold,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Ogroid,Categorie.Token},//需要添加
                    Flavor = "其他巨魔都觉得他是个异类，毕竟在巨魔们看来，谁会喜欢彩色纸片胜过喜欢石头呢？",
                    Info = "派“店店”去班·阿德学院，见见那里的小伙子们。 抽1张牌；随机魅惑1个敌军单位；在对方三排随机生成一种灾厄 ；对1个敌军造成10点伤害，再对其相邻单位造成5点；从牌组打出1张铜色/银色“特殊”牌。",
                    CardArtsId = "20172500",
                }
            },
            {
                "15004",//梦魇独角兽
                new GwentCard()
                {
                    CardId ="15004",
                    EffectType=typeof(Chironex),//效果
                    Name="梦魇独角兽",
                    Strength=4,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Token},//需要添加
                    Flavor = "“天呐，那根本不是独角兽！那是……”——著名奇珍收藏家崴尔玛的遗言。",
                    Info = "对所有其他单位造成2点伤害。",
                    CardArtsId = "11240200",
                }
            },
            {
                "15005",//翡翠人偶
                new GwentCard()
                {
                    CardId ="15005",
                    EffectType=typeof(NoneEffect),//效果
                    Name="翡翠人偶",
                    Strength=2,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Token},//需要添加
                    Flavor = "雕像瞬间爆开，颤动不已，犹如一道在地上爬行的烟雾，变换着自己的形状。道道光芒里，有东西上下纷飞，不断成形。片刻之后，魔法圈的正中间突然现出了一道人影。",
                    Info = "没有特殊技能。",
                    CardArtsId = "20005300",
                }
            },
            {
                "15006",//话篓子：捣蛋鬼
                new GwentCard()
                {
                    CardId ="15006",
                    EffectType=typeof(FieldMarshalDudaAgitator),//效果
                    Name="话篓子：捣蛋鬼",
                    Strength=1,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Token},//需要添加
                    Flavor = "卓尔坦的鹦鹉拥有一项超凡能力：能逼疯所有与它相处的人，包括卓尔坦本人。",
                    Info = "对左右各2格内的单位造成2点伤害。",
                    CardArtsId = "11240400",
                }
            },
            {
                "15007",//话篓子：伙伴
                new GwentCard()
                {
                    CardId ="15007",
                    EffectType=typeof(FieldMarshalDudaCompanion),//效果
                    Name="话篓子：伙伴",
                    Strength=1,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Token},//需要添加
                    Flavor = "它会说一百个词，其中八十个是脏话，剩下的是脏话的语气词。",
                    Info = "使左右各2格内的单位获得2点增益。",
                    CardArtsId = "11240300",
                }
            },
            {
                "15008",//独角兽
                new GwentCard()
                {
                    CardId ="15008",
                    EffectType=typeof(Unicorn),//效果
                    Name="独角兽",
                    Strength=1,
                    Group=Group.Silver,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Token},//需要添加
                    Flavor = "都说独角兽喜欢幼莲。但如今，幼莲跟独角兽一样稀缺，这理论也就难以证明了。",
                    Info = "使所有其他单位获得2点增益。",
                    CardArtsId = "11240100",
                }
            },
            {
                "15009",//羊角魔
                new GwentCard()
                {
                    CardId ="15009",
                    EffectType=typeof(NoneEffect),//效果
                    Name="羊角魔",
                    Strength=14,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Relict,Categorie.Token},//需要添加
                    Flavor = "牛牛防卫队的一员。永远忠诚！",
                    Info = "没有特殊技能。",
                    CardArtsId = "20032000",
                }
            },
            {
                "15010",//熊
                new GwentCard()
                {
                    CardId ="15010",
                    EffectType=typeof(NoneEffect),//效果
                    Name="熊",
                    Strength=11,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Cursed},//需要添加
                    Flavor = "去猎熊吧！去抓上一只——这、这只也太大了吧！快跑！！",
                    Info = "没有特殊技能。",
                    CardArtsId = "15240600",
                }
            },
            {
                "15011",//农民
                new GwentCard()
                {
                    CardId ="15011",
                    EffectType=typeof(NoneEffect),//效果
                    Name="农民",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Doomed},//需要添加
                    Flavor = "瞧，我们是民兵。我们保卫和平",
                    Info = "没有特殊技能。",
                    CardArtsId = "20167700",
                }
            },
            {
                "15012",//牛
                new GwentCard()
                {
                    CardId ="15012",
                    EffectType=typeof(NoneEffect),//效果
                    Name="牛",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Token},//需要添加
                    Flavor = "在鲍克兰，什么东西都比其它地方的要棒：酒更甜美，牛儿更肥，而姑娘们则更加动人。",
                    Info = "没有特殊技能。",
                    CardArtsId = "20051700",
                }
            },
            {
                "15013",//晴空
                new GwentCard()
                {
                    CardId ="15013",
                    EffectType=typeof(ClearSkies),//效果
                    Name="晴空",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special,Categorie.Token},//需要添加
                    Flavor = "太阳出来了，德洛米！太阳出来了！也许我们命不该绝……",
                    Info = "使灾厄下的所有受伤友军单位获得2点增益，并清除己方半场所有灾厄。",
                    CardArtsId = "11330300",
                }
            },
            {
                "15014",//重整
                new GwentCard()
                {
                    CardId ="15014",
                    EffectType=typeof(Rally),//效果
                    Name="重整",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Neutral,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special,Categorie.Token},//需要添加
                    Flavor = "太阳出来了，德洛米！太阳出来了！也许我们命不该绝……",
                    Info = "从己方牌组打出1张随机铜色单位牌。",
                    CardArtsId = "11330300",
                }
            },
            {
                "21001",//达冈
                new GwentCard()
                {
                    CardId ="21001",
                    EffectType=typeof(Dagon),//效果
                    Name="达冈",
                    Strength=8,
                    Group=Group.Leader,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Vodyanoi},//需要添加
                    Flavor = "永世长眠者未必永恒死亡，在奇妙的时代死亡亦将消逝。",
                    Info = "生成“蔽日浓雾”或“倾盆大雨”。",
                    CardArtsId = "13110300",
                }
            },
            {
                "21002",//蟹蜘蛛女王
                new GwentCard()
                {
                    CardId ="21002",
                    EffectType=typeof(ArachasQueen),//效果
                    Name="蟹蜘蛛女王",
                    Strength=7,
                    Group=Group.Leader,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Insectoid},//需要添加
                    Flavor = "她的孩子们完美继承了她的美貌。",
                    Info = "吞噬3个友军单位，获得其战力作为增益。 免疫。",
                    CardArtsId = "20174300",
                }
            },
            {
                "21003",//呢喃山丘
                new GwentCard()
                {
                    CardId ="21003",
                    EffectType=typeof(WhisperingHillock),//效果
                    Name="呢喃山丘",
                    Strength=6,
                    Group=Group.Leader,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Relict},//需要添加
                    Flavor = "它会在其它我们所无法触及的地方再次崛起。厄运会再次降临。",
                    Info = "创造1张铜色/银色“有机”牌。",
                    CardArtsId = "20158700",
                }
            },
            {
                "21004",//艾瑞汀
                new GwentCard()
                {
                    CardId ="21004",
                    EffectType=typeof(EredinBrAccGlas),//效果
                    Name="艾瑞汀",
                    Strength=5,
                    Group=Group.Leader,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.WildHunt,Categorie.Leader},//需要添加
                    Flavor = "留点尊严吧，你的结局已然注定。",
                    Info = "生成1个铜色“狂猎”单位。",
                    CardArtsId = "13110100",
                }
            },
            {
                "21005",//暗影长者
                new GwentCard()
                {
                    CardId ="21005",
                    EffectType=typeof(UnseenElder),//效果
                    Name="暗影长者",
                    Strength=5,
                    Group=Group.Leader,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Vampire},//需要添加
                    Flavor = "没有谁知道暗影长者的真实年龄，连高阶吸血鬼们也不知情。他们唯一清楚的是，无论如何也不能违背他的意愿。",
                    Info = "汲食1个单位一半的战力。",
                    CardArtsId = "20005500",
                }
            },
            {
                "22001",//老矛头：昏睡
                new GwentCard()
                {
                    CardId ="22001",
                    EffectType=typeof(OldSpeartipAsleep),//效果
                    Name="老矛头：昏睡",
                    Strength=12,
                    Group=Group.Gold,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Ogroid},//需要添加
                    Flavor = "别吵！",
                    Info = "使手牌、牌组和己方半场除自身外所有“食人魔”单位获得1点强化。",
                    CardArtsId = "13221800",
                }
            },
            {
                "22002",//战灵
                new GwentCard()
                {
                    CardId ="22002",
                    EffectType=typeof(Draug),//效果
                    Name="战灵",
                    Strength=10,
                    Group=Group.Gold,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Officer},//需要添加
                    Flavor = "有些人就是不服输，死了还要继续打。",
                    Info = "将死去的单位复活为战力为1的“战鬼”，直至填满此排。",
                    CardArtsId = "13210100",
                }
            },
            {
                "22003",//老矛头
                new GwentCard()
                {
                    CardId ="22003",
                    EffectType=typeof(OldSpeartip),//效果
                    Name="老矛头",
                    Strength=10,
                    Group=Group.Gold,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Ogroid},//需要添加
                    Flavor = "哦，你现在可有大麻烦了......",
                    Info = "对最多5个敌军同排单位造成2点伤害。",
                    CardArtsId = "13240800",
                }
            },
            {
                "22004",//卡兰希尔
                new GwentCard()
                {
                    CardId ="22004",
                    EffectType=typeof(CaranthirArFeiniel),//效果
                    Name="卡兰希尔",
                    Strength=9,
                    Group=Group.Gold,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.WildHunt,Categorie.Mage,Categorie.Officer},//需要添加
                    Flavor = "宠儿偏偏堕落成逆子。",
                    Info = "将1个敌军单位移至对方同排，并在此排降下“刺骨冰霜”。",
                    CardArtsId = "13210400",
                }
            },
            {
                "22005",//伊勒瑞斯
                new GwentCard()
                {
                    CardId ="22005",
                    EffectType=typeof(Imlerith),//效果
                    Name="伊勒瑞斯",
                    Strength=9,
                    Group=Group.Gold,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.WildHunt,Categorie.Officer},//需要添加
                    Flavor = "叫他们有来无回！",
                    Info = "对1个敌军单位造成4点伤害，若目标位于“刺骨冰霜”之下，则伤害变为8点。",
                    CardArtsId = "13210200",
                }
            },
            {
                "22006",//呢喃婆：贡品
                new GwentCard()
                {
                    CardId ="22006",
                    EffectType=typeof(WhispessTribute),//效果
                    Name="呢喃婆：贡品",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Relict},//需要添加
                    Flavor = "我将是你最美……与最后的体验。",
                    Info = "从牌组打出1张铜色/银色“有机”牌。",
                    CardArtsId = "20022200",
                }
            },
            {
                "22007",//巨章鱼怪
                new GwentCard()
                {
                    CardId ="22007",
                    EffectType=typeof(Kayran),//效果
                    Name="巨章鱼怪",
                    Strength=5,
                    Group=Group.Gold,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Insectoid},//需要添加
                    Flavor = "对付巨章鱼怪？简单。找出你最好的剑——卖了它，雇个猎魔人。",
                    Info = "吞噬1个战力不高于7点的单位，将其战力传化为自身增益。",
                    CardArtsId = "13210700",
                }
            },
            {
                "22008",//林妖
                new GwentCard()
                {
                    CardId ="22008",
                    EffectType=typeof(WoodlandSpirit),//效果
                    Name="林妖",
                    Strength=5,
                    Group=Group.Gold,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Relict},//需要添加
                    Flavor = "即便全村人都饿肚子，我们也绝不去这片树林打猎。",
                    Info = "在近战排生成3只“狼”，并在对方同排降下“蔽日浓雾”。",
                    CardArtsId = "13210300",
                }
            },
            {
                "22009",//伊勒瑞斯：临终之日
                new GwentCard()
                {
                    CardId ="22009",
                    EffectType=typeof(ImlerithSabbath),//效果
                    Name="伊勒瑞斯：临终之日",
                    Strength=5,
                    Group=Group.Gold,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.WildHunt,Categorie.Officer},//需要添加
                    Flavor = "老巫妪说过你会来。她们在水面上预见了你的到来。",
                    Info = "2点护甲。 每回合结束时，与最强的敌军单位对决。若存活，则回复2点战力并获得2点护甲。",
                    CardArtsId = "20178100",
                }
            },
            {
                "22010",//看门人
                new GwentCard()
                {
                    CardId ="22010",
                    EffectType=typeof(Caretaker),//效果
                    Name="看门人",
                    Strength=4,
                    Group=Group.Gold,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Doomed,Categorie.Relict},//需要添加
                    Flavor = "大千世界，无奇不有，远超古圣今贤之想象。",
                    Info = "从对方墓场复活1个铜色/银色单位。",
                    CardArtsId = "13210600",
                }
            },
            {
                "22011",//米卢娜
                new GwentCard()
                {
                    CardId ="22011",
                    EffectType=typeof(Miruna),//效果
                    Name="米卢娜",
                    Strength=4,
                    Group=Group.Gold,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast},//需要添加
                    Flavor = "打什么仗嘛？明明有更多好办法消耗过剩的精力……",
                    Info = "2回合后的回合开始时：魅惑对方同排最强的单位。",
                    CardArtsId = "13210800",
                }
            },
            {
                "22012",//织婆：咒文
                new GwentCard()
                {
                    CardId ="22012",
                    EffectType=typeof(WeavessIncantation),//效果
                    Name="织婆：咒文",
                    Strength=4,
                    Group=Group.Gold,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Relict},//需要添加
                    Flavor = "我能感受到你的痛苦和恐惧。",
                    Info = "择一：使位于手牌、牌组和己方半场除自身外的所有“残物”单位获得2点强化；或从牌组打出1张铜色/银色“残物” 牌，并使其获得2点强化。",
                    CardArtsId = "20022000",
                }
            },
            {
                "22013",//盖尔
                new GwentCard()
                {
                    CardId ="22013",
                    EffectType=typeof(GeEls),//效果
                    Name="盖尔",
                    Strength=1,
                    Group=Group.Gold,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.WildHunt,Categorie.Officer},//需要添加
                    Flavor = "画作传情，而非言词。",
                    Info = "检视牌组中1张金色牌和银色牌，打出1张，将另1张置于牌组顶端。",
                    CardArtsId = "13110200",
                }
            },
            {
                "22014",//煮婆：仪式
                new GwentCard()
                {
                    CardId ="22014",
                    EffectType=typeof(BrewessRitual),//效果
                    Name="煮婆：仪式",
                    Strength=1,
                    Group=Group.Gold,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Doomed,Categorie.Relict},//需要添加
                    Flavor = "切烂……剁碎……然后熬出……一锅好汤。",
                    Info = "复活2个铜色遗愿单位。",
                    CardArtsId = "20022100",
                }
            },
            {
                "23001",//畏惧者
                new GwentCard()
                {
                    CardId ="23001",
                    EffectType=typeof(Frightener),//效果
                    Name="畏惧者",
                    Strength=13,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Construct,Categorie.Agent},//需要添加
                    Flavor = "“我到底做了什么？”法师被自己的创造物吓得大叫起来。",
                    Info = "间谍、力竭。 将1个敌军单位移至自身所在排，然后抽1张牌。",
                    CardArtsId = "13220400",
                }
            },
            {
                "23002",//帝国蝎尾狮
                new GwentCard()
                {
                    CardId ="23002",
                    EffectType=typeof(NoneEffect),//效果
                    Name="帝国蝎尾狮",
                    Strength=13,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast},//需要添加
                    Flavor = "世界上最古老且致命的怪物之一。倘若是过去，遇到一只定会让我兴奋无比。但如今它只是我前进路上的绊脚石，而它的肉和滚烫的鲜血能帮我熬过这冰天雪地。",
                    Info = "没有特殊技能。",
                    CardArtsId = "13220900",
                }
            },
            {
                "23003",//哥亚特
                new GwentCard()
                {
                    CardId ="23003",
                    EffectType=typeof(Golyat),//效果
                    Name="哥亚特",
                    Strength=10,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Ogroid},//需要添加
                    Flavor = "据说哥亚特曾经是一位闻名遐迩的骑士。只可惜，有一天他触怒了湖中仙女，被变成了一头怪物。",
                    Info = "获得7点增益。 每次被伤害时，额外承受2点伤害。",
                    CardArtsId = "20005200",
                }
            },
            {
                "23004",//煮婆
                new GwentCard()
                {
                    CardId ="23004",
                    EffectType=typeof(Brewess),//效果
                    Name="煮婆",
                    Strength=8,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Relict},//需要添加
                    Flavor = "切烂……剁碎……然后熬出……一锅好汤。",
                    Info = "召唤“呢喃婆”和“织婆”。",
                    CardArtsId = "13220700",
                }
            },
            {
                "23005",//伊夫利特
                new GwentCard()
                {
                    CardId ="23005",
                    EffectType=typeof(Ifrit),//效果
                    Name="伊夫利特",
                    Strength=8,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Construct},//需要添加
                    Flavor = "受不了热浪？那就没生路了。",
                    Info = "在右侧生成3个“次级伊夫利特”。",
                    CardArtsId = "13221000",
                }
            },
            {
                "23006",//卢恩
                new GwentCard()
                {
                    CardId ="23006",
                    EffectType=typeof(Ruehin),//效果
                    Name="卢恩",
                    Strength=8,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Insectoid,Categorie.Cursed},//需要添加
                    Flavor = "进入那片森林的人，没一个活着回来……",
                    Info = "使位于手牌、牌组和己方半场自身外的所有“类虫生物”和“诅咒生物”单位获得1点强化。",
                    CardArtsId = "4010200",
                }
            },
            {
                "23007",//织婆
                new GwentCard()
                {
                    CardId ="23007",
                    EffectType=typeof(Weavess),//效果
                    Name="织婆",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Relict},//需要添加
                    Flavor = "我能感受到你的痛苦和恐惧。",
                    Info = "召唤“呢喃婆”和“煮婆”。",
                    CardArtsId = "13220800",
                }
            },
            {
                "23008",//呢喃婆
                new GwentCard()
                {
                    CardId ="23008",
                    EffectType=typeof(Whispess),//效果
                    Name="呢喃婆",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Relict},//需要添加
                    Flavor = "我将是你最美……与最后的体验。",
                    Info = "召唤“煮婆”和“织婆”。",
                    CardArtsId = "13220600",
                }
            },
            {
                "23009",//约顿
                new GwentCard()
                {
                    CardId ="23009",
                    EffectType=typeof(Jotunn),//效果
                    Name="约顿",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Ogroid},//需要添加
                    Flavor = "在史凯利格的传说中，强大而恐怖的巨人之王约顿是群岛在上古时期的统治者。他最终死于汉姆多尔的剑下，但在弥留之际，他发誓要在终末之战时重返人间。",
                    Info = "将3个敌军单位移至对方同排，并对它们造成2点伤害。若该排上有“刺骨冰霜”生效，则将伤害提高至3点。",
                    CardArtsId = "20021800",
                }
            },
            {
                "23010",//莫伍德
                new GwentCard()
                {
                    CardId ="23010",
                    EffectType=typeof(Morvudd),//效果
                    Name="莫伍德",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Relict},//需要添加
                    Flavor = "一种只在大史凯利格出没的珍稀鹿首魔品种。对食物异乎寻常的挑剔。",
                    Info = "改变1个单位的锁定状态。若目标为敌军，则使其战力减半。",
                    CardArtsId = "13220500",
                }
            },
            {
                "23011",//尼斯里拉
                new GwentCard()
                {
                    CardId ="23011",
                    EffectType=typeof(Nithral),//效果
                    Name="尼斯里拉",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.WildHunt,Categorie.Officer},//需要添加
                    Flavor = "每个狂猎战士都要经过严格筛选，而只有最残暴、最凶狠的那些才能加入艾瑞汀的骑兵队。",
                    Info = "对1个敌军单位造成6点伤害。手牌中每有1张“狂猎”单位牌，伤害提高1点。",
                    CardArtsId = "13221400",
                }
            },
            {
                "23012",//蟾蜍王子
                new GwentCard()
                {
                    CardId ="23012",
                    EffectType=typeof(ToadPrince),//效果
                    Name="蟾蜍王子",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed},//需要添加
                    Flavor = "又大又坏又丑，住在下水道里。",
                    Info = "抽1张单位牌，随后吞噬1张手牌中的单位牌，获得等同于其战力的增益。",
                    CardArtsId = "13221600",
                }
            },
            {
                "23013",//吸血妖鸟
                new GwentCard()
                {
                    CardId ="23013",
                    EffectType=typeof(Striga),//效果
                    Name="吸血妖鸟",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Relict},//需要添加
                    Flavor = "吸血妖鸟挺好的。当然，她会时不时地在谁的身上咬一口，但我们早就习惯了。",
                    Info = "对1个非“怪兽”单位造成8点伤害。",
                    CardArtsId = "20007300",
                }
            },
            {
                "23014",//吸血夜魔
                new GwentCard()
                {
                    CardId ="23014",
                    EffectType=typeof(Nekurat),//效果
                    Name="吸血夜魔",
                    Strength=5,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Vampire},//需要添加
                    Flavor = "天球交汇后，这些以鲜血为食的怪物便来到了我们的世界。",
                    Info = "生成“月光”。",
                    CardArtsId = "13222000",
                }
            },
            {
                "23015",//欧兹瑞尔
                new GwentCard()
                {
                    CardId ="23015",
                    EffectType=typeof(Ozzrel),//效果
                    Name="欧兹瑞尔",
                    Strength=5,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Necrophage},//需要添加
                    Flavor = "一些食腐生物袭击过人类后，便再也不满足于区区腐肉了……",
                    Info = "吞噬双方墓场中1个铜色/银色单位，获得其战力作为增益。",
                    CardArtsId = "20169800",
                }
            },
            {
                "23016",//阿巴亚
                new GwentCard()
                {
                    CardId ="23016",
                    EffectType=typeof(Abaya),//效果
                    Name="阿巴亚",
                    Strength=4,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Necrophage},//需要添加
                    Flavor = "丑东西我见得多了，海鳝、七鳃鳗、水滴鱼……但还没见过这么丑的！",
                    Info = "生成“倾盆大雨”、“晴空”或“蟹蜘蛛毒液”。",
                    CardArtsId = "13220300",
                }
            },
            {
                "23017",//莫恩塔特
                new GwentCard()
                {
                    CardId ="23017",
                    EffectType=typeof(Mourntart),//效果
                    Name="莫恩塔特",
                    Strength=4,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Necrophage},//需要添加
                    Flavor = "大部分墓穴巫婆以墓穴中刨出来的尸体为食。但有些也会潜入小屋，偷走孩子，残害大人。因此基本上没人愿意和她们成为邻居。",
                    Info = "吞噬己方墓场所有铜色/银色单位。每吞噬1个单位，便获得1点增益。",
                    CardArtsId = "13220200",
                }
            },
            {
                "23018",//无骨者
                new GwentCard()
                {
                    CardId ="23018",
                    EffectType=typeof(Maerolorn),//效果
                    Name="无骨者",
                    Strength=4,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Relict},//需要添加
                    Flavor = "别怕阴影，该怕的是亮光。",
                    Info = "从牌组打出1张拥有遗愿能力的铜色单位牌。",
                    CardArtsId = "13240600",
                }
            },
            {
                "23019",//维尔金的女巨魔
                new GwentCard()
                {
                    CardId ="23019",
                    EffectType=typeof(SheTrollOfVergen),//效果
                    Name="维尔金的女巨魔",
                    Strength=1,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Ogroid},//需要添加
                    Flavor = "着迷于精灵洋葱汤。",
                    Info = "从牌组打出1个铜色“遗愿”单位。吞噬它并获得其基础战力的增益。",
                    CardArtsId = "20048200",
                }
            },
            {
                "23020",//戴维娜符文石
                new GwentCard()
                {
                    CardId ="23020",
                    EffectType=typeof(DevenaRunestone),//效果
                    Name="戴维娜符文石",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "我的剑实在是锋利，连纸都能裁开！",
                    Info = "创造1张铜色/银色“怪兽”牌。",
                    CardArtsId = "20158400",
                }
            },
            {
                "23021",//怪物巢穴
                new GwentCard()
                {
                    CardId ="23021",
                    EffectType=typeof(MonsterNest),//效果
                    Name="怪物巢穴",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Organic},//需要添加
                    Flavor = "该死，这些怪物泛滥成灾……我们得找个猎魔人，不然大家都活不成。",
                    Info = "生成1个铜色“食腐生物”或“类虫生物”单位，使其获得1点增益。",
                    CardArtsId = "13330200",
                }
            },
            {
                "23022",//寄生虫
                new GwentCard()
                {
                    CardId ="23022",
                    EffectType=typeof(Parasite),//效果
                    Name="寄生虫",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Organic},//需要添加
                    Flavor = "确实引人注目。",
                    Info = "对1个敌军单位造成12点伤害；或使1个友军单位获得12点增益。",
                    CardArtsId = "20153400",
                }
            },
            {
                "24001",//独眼巨人
                new GwentCard()
                {
                    CardId ="24001",
                    EffectType=typeof(Cyclops),//效果
                    Name="独眼巨人",
                    Strength=11,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Ogroid},//需要添加
                    Flavor = "别盯着它的眼睛，他不喜欢那样……",
                    Info = "摧毁1个友军单位，对1个敌军单位造成等同于被摧毁友军单位战力的伤害。",
                    CardArtsId = "20003700",
                }
            },
            {
                "24002",//鹿首魔
                new GwentCard()
                {
                    CardId ="24002",
                    EffectType=typeof(NoneEffect),//效果
                    Name="鹿首魔",
                    Strength=11,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Relict},//需要添加
                    Flavor = "鹿首魔长得有点像鹿，一只又大又邪恶的鹿。",
                    Info = "没有特殊技能。",
                    CardArtsId = "11240500",
                }
            },
            {
                "24003",//远古小雾妖
                new GwentCard()
                {
                    CardId ="24003",
                    EffectType=typeof(AncientFoglet),//效果
                    Name="远古小雾妖",
                    Strength=10,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Necrophage},//需要添加
                    Flavor = "人类心中潜藏着许多原始的恐惧。对迷雾的恐惧更是根深蒂固……",
                    Info = "回合结束时，若场上任意位置有“蔽日浓雾”，则获得1点增益。",
                    CardArtsId = "13230200",
                }
            },
            {
                "24004",//大狮鹫
                new GwentCard()
                {
                    CardId ="24004",
                    EffectType=typeof(Archgriffin),//效果
                    Name="大狮鹫",
                    Strength=10,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast},//需要添加
                    Flavor = "大狮鹫也是狮鹫，只是更加凶悍。",
                    Info = "移除所在排的灾厄。",
                    CardArtsId = "13231200",
                }
            },
            {
                "24005",//狂猎骑士
                new GwentCard()
                {
                    CardId ="24005",
                    EffectType=typeof(WildHuntRider),//效果
                    Name="狂猎骑士",
                    Strength=10,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.WildHunt,Categorie.Soldier},//需要添加
                    Flavor = "首先映入眼帘的是头盔旁的两只水牛角，接着是牛角间的头冠，最后是面甲下白骨般的脸。",
                    Info = "使对方同排的“刺骨冰霜”伤害提高1点。",
                    CardArtsId = "13231010",
                }
            },
            {
                "24006",//守桥巨魔
                new GwentCard()
                {
                    CardId ="24006",
                    EffectType=typeof(BridgeTroll),//效果
                    Name="守桥巨魔",
                    Strength=10,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Ogroid},//需要添加
                    Flavor = "桥桥？巨魔造，很久很久前。",
                    Info = "将对方半场上的1个灾厄效果移至另一排。",
                    CardArtsId = "20048100",
                }
            },
            {
                "24007",//狼人头领
                new GwentCard()
                {
                    CardId ="24007",
                    EffectType=typeof(AlphaWerewolf),//效果
                    Name="狼人头领",
                    Strength=10,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Cursed},//需要添加
                    Flavor = "有人说，如果被狼人咬了，那么你就会被感染，也变成狼人。当然，猎魔人都知道这是胡说八道。只有强大的诅咒才能造成这种效果。",
                    Info = "接触“满月”效果时，在自身两侧各生成1只“狼”。",
                    CardArtsId = "20011400",
                }
            },
            {
                "24008",//狮鹫
                new GwentCard()
                {
                    CardId ="24008",
                    EffectType=typeof(Griffin),//效果
                    Name="狮鹫",
                    Strength=9,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast},//需要添加
                    Flavor = "狮鹫喜欢玩弄自己的猎物，将其一点一点地活生生啄食吞咽。",
                    Info = "触发1个铜色友军单位的遗愿效果。",
                    CardArtsId = "13230700",
                }
            },
            {
                "24009",//孽鬼战士
                new GwentCard()
                {
                    CardId ="24009",
                    EffectType=typeof(NekkerWarrior),//效果
                    Name="孽鬼战士",
                    Strength=9,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Ogroid},//需要添加
                    Flavor = "各位小心，这座桥下有孽鬼出没。",
                    Info = "选择1个友军铜色单位，将2张它的同名牌加入牌组底部。",
                    CardArtsId = "13221100",
                }
            },
            {
                "24010",//蟹蜘蛛巨兽
                new GwentCard()
                {
                    CardId ="24010",
                    EffectType=typeof(ArachasBehemoth),//效果
                    Name="蟹蜘蛛巨兽",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Insectoid},//需要添加
                    Flavor = "看起来像螃蟹和蜘蛛的杂交……只是体型硕大无比。",
                    Info = "每当友军单位吞噬1个单位，便在随机排生成1只“蟹蜘蛛幼虫”。 一共可生效4次。",
                    CardArtsId = "13220100",
                }
            },
            {
                "24011",//腐食魔
                new GwentCard()
                {
                    CardId ="24011",
                    EffectType=typeof(Rotfiend),//效果
                    Name="腐食魔",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Necrophage},//需要添加
                    Flavor = "还没见到它们的踪影，恶臭就先远远地传过来了。",
                    Info = "遗愿：对对方同排所有单位造成2点伤害。",
                    CardArtsId = "20029500",
                }
            },
            {
                "24012",//叉尾龙
                new GwentCard()
                {
                    CardId ="24012",
                    EffectType=typeof(Forktail),//效果
                    Name="叉尾龙",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Draconid},//需要添加
                    Flavor = "要想印叉尾龙上钩，得这样做：在地里打一根桩子，上头绑一只山羊，然后赶紧钻到旁边的灌木丛里藏起来。",
                    Info = "吞噬2个友军单位，并获得其战力的增益。",
                    CardArtsId = "20141500",
                }
            },
            {
                "24013",//巨棘魔树
                new GwentCard()
                {
                    CardId ="24013",
                    EffectType=typeof(Archespore),//效果
                    Name="巨棘魔树",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed},//需要添加
                    Flavor = "根据民间传说，它们会在亡者鲜血的浇灌下破土而出。因此在拥有黑暗过往的地方，它们长得特别茂盛。",
                    Info = "回合开始时移至随机排，对1个随机敌军单位造成1点伤害。 遗愿：对1个敌军随机单位造成4点伤害。",
                    CardArtsId = "20003800",
                }
            },
            {
                "24014",//水鬼
                new GwentCard()
                {
                    CardId ="24014",
                    EffectType=typeof(Drowner),//效果
                    Name="水鬼",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Necrophage},//需要添加
                    Flavor = "尽管猎魔人想多赚些金币，但杀水鬼这活儿只值一枚银币，或者三个铜板——不能再多了。",
                    Info = "将1个敌军单位拖至对方同排，对其造成2点伤害，若目标排处于灾厄之下，则伤害提高至4点。",
                    CardArtsId = "13231400",
                }
            },
            {
                "24015",//狂猎长船
                new GwentCard()
                {
                    CardId ="24015",
                    EffectType=typeof(WildHuntDrakkar),//效果
                    Name="狂猎长船",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.WildHunt,Categorie.Machine},//需要添加
                    Flavor = "鬼船纳吉尔法的船头劈开波浪，将海水破成两道。号角响彻天际，汉姆多尔站在燃烧的彩虹之上，迎接敌人的来犯。白霜降临，狂风和暴雨近在咫尺……",
                    Info = "使所有友军“狂猎”单位获得1点增益。 后续出现的其他友军“狂猎”单位也将获得1点增益。",
                    CardArtsId = "20030100",
                }
            },
            {
                "24016",//狂猎战士
                new GwentCard()
                {
                    CardId ="24016",
                    EffectType=typeof(WildHuntWarrior),//效果
                    Name="狂猎战士",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.WildHunt,Categorie.Soldier},//需要添加
                    Flavor = "白霜将至。",
                    Info = "对1个敌军单位造成3点伤害。若目标位于“刺骨冰霜”之下或被摧毁，则获得2点增益。",
                    CardArtsId = "13230900",
                }
            },
            {
                "24017",//狼人
                new GwentCard()
                {
                    CardId ="24017",
                    EffectType=typeof(Werewolf),//效果
                    Name="狼人",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Cursed},//需要添加
                    Flavor = "有人说，如果被狼人咬了，那么你就会被感染，也变成狼人。当然，猎魔人都知道这是胡说八道。只有强大的诅咒才能造成这种效果。",
                    Info = "接触“满月”后获得7点增益。 免疫。",
                    CardArtsId = "20009900",
                }
            },
            {
                "24018",//赛尔伊诺鹰身女妖
                new GwentCard()
                {
                    CardId ="24018",
                    EffectType=typeof(CelaenoHarpy),//效果
                    Name="赛尔伊诺鹰身女妖",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast},//需要添加
                    Flavor = "一般的鹰身女妖以腐肉为食，赛尔伊诺鹰身女妖……则以梦境为食。",
                    Info = "在左侧生成2枚“鹰身女妖蛋”。",
                    CardArtsId = "13221700",
                }
            },
            {
                "24019",//石化鸡蛇
                new GwentCard()
                {
                    CardId ="24019",
                    EffectType=typeof(Cockatrice),//效果
                    Name="石化鸡蛇",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Draconid},//需要添加
                    Flavor = "头部拥有超强的防抖能力，因此总能轻易命中目标的腰椎之间、左肾下面，或是主动脉。",
                    Info = "重置一个单位。",
                    CardArtsId = "20023300",
                }
            },
            {
                "24020",//地灵
                new GwentCard()
                {
                    CardId ="24020",
                    EffectType=typeof(DAo),//效果
                    Name="地灵",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Construct},//需要添加
                    Flavor = "怎么跟土元素打？別想了，跑吧，能跑多快跑多快。",
                    Info = "遗愿：在同排生成2个“次级地灵”。",
                    CardArtsId = "13221300",
                }
            },
            {
                "24021",//寒冰巨人
                new GwentCard()
                {
                    CardId ="24021",
                    EffectType=typeof(IceGiant),//效果
                    Name="寒冰巨人",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Ogroid},//需要添加
                    Flavor = "我这辈子只当过一次逃兵，就是碰上寒冰巨人那次——我一点也没觉得丢人。",
                    Info = "若场上任意位置有“刺骨冰霜”，则获得6点增益。",
                    CardArtsId = "13221200",
                }
            },
            {
                "24022",//蜥蜴人战士
                new GwentCard()
                {
                    CardId ="24022",
                    EffectType=typeof(VranWarrior),//效果
                    Name="蜥蜴人战士",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    Countdown = 2, 
                    IsDoomed = false,
                    IsCountdown = true,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Draconid},//需要添加
                    Flavor = "他们静静地骑在马上，看起来很放松，但全副武装：宽头短矛、剑柄独特的剑、战斧以及锯齿长斧。",
                    Info = "吞噬右侧单位，获得其战力作为增益。 每2回合开始时，重复此能力。",
                    CardArtsId = "13230800",
                }
            },
            {
                "24023",//翼手龙
                new GwentCard()
                {
                    CardId ="24023",
                    EffectType=typeof(Wyvern),//效果
                    Name="翼手龙",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Draconid},//需要添加
                    Flavor = "想象一下在最可怕的噩梦中出现的长翅膀的蛇——翼手龙比这更可怕。",
                    Info = "对1个敌军单位造成5点伤害。",
                    CardArtsId = "13230300",
                }
            },
            {
                "24024",//女蛇妖
                new GwentCard()
                {
                    CardId ="24024",
                    EffectType=typeof(Lamia),//效果
                    Name="女蛇妖",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast},//需要添加
                    Flavor = "有个迷信的家伙用蜡堵住耳朵，结果什么也听不到，包括警告——他的船直接撞上了礁石。",
                    Info = "对1个敌军单位造成4点伤害，若目标位于“血月”之下，则伤害变为7点。",
                    CardArtsId = "20011300",
                }
            },
            {
                "24025",//须岩怪
                new GwentCard()
                {
                    CardId ="24025",
                    EffectType=typeof(Barbegazi),//效果
                    Name="须岩怪",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Insectoid},//需要添加
                    Flavor = "岩洞中先前还和石头没什么两样的怪物，倏地瞪大眼睛，充满恶意地盯着他。",
                    Info = "坚韧。 吞噬1个友军单位，获得其战力作为增益。",
                    CardArtsId = "20170100",
                }
            },
            {
                "24026",//蝙魔
                new GwentCard()
                {
                    CardId ="24026",
                    EffectType=typeof(Ekimmara),//效果
                    Name="蝙魔",
                    Strength=5,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Vampire},//需要添加
                    Flavor = "谁能想到巨型蝙蝠也会喜欢俗艳的珠宝？",
                    Info = "汲食1个单位3点战力。",
                    CardArtsId = "13231300",
                }
            },
            {
                "24027",//猫人
                new GwentCard()
                {
                    CardId ="24027",
                    EffectType=typeof(Werecat),//效果
                    Name="猫人",
                    Strength=5,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Cursed},//需要添加
                    Flavor = "他不喜欢你挠他的肚子。",
                    Info = "对1个敌军单位造成5点伤害，随后对位于“血月”之下的所有敌军单位造成1点伤害。",
                    CardArtsId = "13240900",
                }
            },
            {
                "24028",//小雾妖
                new GwentCard()
                {
                    CardId ="24028",
                    EffectType=typeof(Foglet),//效果
                    Name="小雾妖",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Necrophage},//需要添加
                    Flavor = "浓雾悄然弥漫时，小雾妖便会出没，来享用它们的受害者。",
                    Info = "每当在对方半场降下“蔽日浓雾”，便召唤1张同名牌至己方同排。",
                    CardArtsId = "13230100",
                }
            },
            {
                "24029",//食尸鬼
                new GwentCard()
                {
                    CardId ="24029",
                    EffectType=typeof(Ghoul),//效果
                    Name="食尸鬼",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Necrophage},//需要添加
                    Flavor = "如果食尸鬼也是生死循环的一环……那这循环也太残酷了。",
                    Info = "吞噬墓场中1个铜色/银色单位，获得等同于其战力的增益。",
                    CardArtsId = "13230600",
                }
            },
            {
                "24030",//鹰身女妖
                new GwentCard()
                {
                    CardId ="24030",
                    EffectType=typeof(Harpy),//效果
                    Name="鹰身女妖",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast},//需要添加
                    Flavor = "鹰身女妖有很多种，每种都有窃盗癖。",
                    Info = "每当1个友军“野兽”单位在己方回合被摧毁，便召唤1张同名牌。",
                    CardArtsId = "13231500",
                }
            },
            {
                "24031",//孽鬼
                new GwentCard()
                {
                    CardId ="24031",
                    EffectType=typeof(Nekker),//效果
                    Name="孽鬼",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Ogroid},//需要添加
                    Flavor = "如果忽略它们会攻击人类的残酷事实，这些小家伙其实挺可爱的。",
                    Info = "若位于手牌、牌组或己方半场：友军单位发动吞噬时获得1点增益。 遗愿：召唤1张同名牌。",
                    CardArtsId = "13230500",
                }
            },
            {
                "24032",//狂猎之犬
                new GwentCard()
                {
                    CardId ="24032",
                    EffectType=typeof(WildHuntHound),//效果
                    Name="狂猎之犬",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.WildHunt,Categorie.Construct},//需要添加
                    Flavor = "下令出动，放狗开战。",
                    Info = "从牌组打出“刺骨冰霜”。",
                    CardArtsId = "13240200",
                }
            },
            {
                "24033",//女海妖
                new GwentCard()
                {
                    CardId ="24033",
                    EffectType=typeof(Siren),//效果
                    Name="女海妖",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast},//需要添加
                    Flavor = "传说她们会用诱人的歌声引水手们上钩……倒不如说，是她们身上一些更加诱人的地方迷住了水手。",
                    Info = "从牌组打出“月光”。",
                    CardArtsId = "20011210",
                }
            },
            {
                "24034",//冰巨魔
                new GwentCard()
                {
                    CardId ="24034",
                    EffectType=typeof(IceTroll),//效果
                    Name="冰巨魔",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Ogroid},//需要添加
                    Flavor = "巨魔形形色色，身材、嗜好各有不同。不过它们的脑子都和一桶锈钉子差不了多少。",
                    Info = "与1个敌军单位对决。若它位于“刺骨冰霜”之下，则己方伤害翻倍。",
                    CardArtsId = "20050200",
                }
            },
            {
                "24035",//蟹蜘蛛雄蛛
                new GwentCard()
                {
                    CardId ="24035",
                    EffectType=typeof(ArachasDrone),//效果
                    Name="蟹蜘蛛雄蛛",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Insectoid},//需要添加
                    Flavor = "丑陋的外表即是警告，叫你“离远点”。",
                    Info = "召唤所有同名牌。",
                    CardArtsId = "13230400",
                }
            },
            {
                "24036",//狂猎导航员
                new GwentCard()
                {
                    CardId ="24036",
                    EffectType=typeof(WildHuntNavigator),//效果
                    Name="狂猎导航员",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.WildHunt,Categorie.Mage},//需要添加
                    Flavor = "几百年来，阿瓦拉克一直试图通过回育之术重塑长者之血的基因——但由此抚育出的精灵孩子不过是萤火灯烛，全然无法与劳拉光辉耀眼的血统相提并论。",
                    Info = "选择1个非“法师”的友军铜色“狂猎”单位，从牌组打出1张它的同名牌。",
                    CardArtsId = "20002600",
                }
            },
            {
                "24037",//飞蜥
                new GwentCard()
                {
                    CardId ="24037",
                    EffectType=typeof(Slyzard),//效果
                    Name="飞蜥",
                    Strength=2,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Draconid},//需要添加
                    Flavor = "不管多强壮的人，不管他的本领多么高强，都不可能在飞蜥尾巴的抽打、巨蝎的大螯、或是狮鹫的爪子下保住性命。",
                    Info = "从己方墓场吞噬1个非同名铜色单位，并从牌组打出1张它的同名牌。",
                    CardArtsId = "20138400",
                }
            },
            {
                "24038",//月光
                new GwentCard()
                {
                    CardId ="24038",
                    EffectType=typeof(Moonlight),//效果
                    Name="月光",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Hazard,Categorie.Boon},//需要添加
                    Flavor = "满月的时候，梦魇便会从世界的各个角落匍匐而出。",
                    Info = "择一：降下“满月”恩泽；或降下“血月”灾厄。",
                    CardArtsId = "20006700",
                }
            },
            {
                "25001",//次级地灵
                new GwentCard()
                {
                    CardId ="25001",
                    EffectType=typeof(NoneEffect),//效果
                    Name="次级地灵",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Construct,Categorie.Token},//需要添加
                    Flavor = "它很容易被误认作岩石，许多糊涂巨魔已痛彻心扉地领悟到这一点。",
                    Info = "没有特殊技能。",
                    CardArtsId = "13240500",
                }
            },
            {
                "25002",//蟹蜘蛛幼虫
                new GwentCard()
                {
                    CardId ="25002",
                    EffectType=typeof(ArachasHatchling),//效果
                    Name="蟹蜘蛛幼虫",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Insectoid,Categorie.Token},//需要添加
                    Flavor = "有时候，“年轻即是美”不过是大自然的谎言罢了。",
                    Info = "召唤所有“蟹蜘蛛雄蛛”。",
                    CardArtsId = "20031000",
                }
            },
            {
                "25003",//战鬼
                new GwentCard()
                {
                    CardId ="25003",
                    EffectType=typeof(NoneEffect),//效果
                    Name="战鬼",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Token},//需要添加
                    Flavor = "战鬼是战之恶魔。它们会出现在极度血腥惨烈的战场上，由万千憎恨凝结而成，嗜血成性。",
                    Info = "没有特殊技能。",
                    CardArtsId = "20045700",
                }
            },
            {
                "25004",//鹰身女妖蛋
                new GwentCard()
                {
                    CardId ="25004",
                    EffectType=typeof(HarpyEgg),//效果
                    Name="鹰身女妖蛋",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Token},//需要添加
                    Flavor = "鹰身女妖蛋卷，真是美味佳肴啊，好先生。但它也非常昂贵，您可能料得到，这些可怜的鸟儿并不愿跟自己的蛋分离。",
                    Info = "使吞噬自身的单位获得额外4点增益。 遗愿：在随机排生成1只“鹰身女妖幼崽”。",
                    CardArtsId = "13231600",
                }
            },
            {
                "25005",//鹰身女妖幼崽
                new GwentCard()
                {
                    CardId ="25005",
                    EffectType=typeof(NoneEffect),//效果
                    Name="鹰身女妖幼崽",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Token},//需要添加
                    Flavor = "有时候，“年轻即是美”不过是大自然的谎言罢了。",
                    Info = "没有特殊技能。",
                    CardArtsId = "20030800",
                }
            },
            {
                "25006",//次级伊夫利特
                new GwentCard()
                {
                    CardId ="25006",
                    EffectType=typeof(LesserIfrit),//效果
                    Name="次级伊夫利特",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Construct,Categorie.Token},//需要添加
                    Flavor = "曾经有个法师觉得它们可爱，而他的命运催生了一句俗语：“不要玩火自焚。”",
                    Info = "对1个敌军随机单位造成1点伤害。",
                    CardArtsId = "13240400",
                }
            },
            {
                "25007",//狼
                new GwentCard()
                {
                    CardId ="25007",
                    EffectType=typeof(NoneEffect),//效果
                    Name="狼",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Token},//需要添加
                    Flavor = "“放心，我知道怎么驯狼……”——猎人邓巴的遗言。",
                    Info = "没有特殊技能。",
                    CardArtsId = "13240300",
                }
            },
            {
                "25008",//血月
                new GwentCard()
                {
                    CardId ="25008",
                    EffectType=typeof(BloodMoon),//效果
                    Name="血月",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Hazard,Categorie.Token},//需要添加
                    Flavor = "满月的时候，梦魇便会从世界的各个角落匍匐而出。",
                    Info = "在对方单排降下灾厄，对该排上所有单位造成2点伤害。",
                    CardArtsId = "20006700",
                }
            },
            {
                "25009",//满月
                new GwentCard()
                {
                    CardId ="25009",
                    EffectType=typeof(FullMoon),//效果
                    Name="满月",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Monsters,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Boon,Categorie.Token},//需要添加
                    Flavor = "满月的时候，梦魇便会从世界的各个角落匍匐而出。",
                    Info = "在己方单排降下恩泽。在回合开始时，使该排上1个随机“野兽”或“吸血鬼”单位获得2点增益。",
                    CardArtsId = "20006700",
                }
            },
            {
                "31002",//恩希尔·恩瑞斯
                new GwentCard()
                {
                    CardId ="31002",
                    EffectType=typeof(EmhyrVarEmreis),//效果
                    Name="恩希尔·恩瑞斯",
                    Strength=7,
                    Group=Group.Leader,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Officer},//需要添加
                    Flavor = "我没什么耐心。把我惹毛了，小心小命不保。",
                    Info = "打出1张手牌，随后将1个友军铜色/银色单位收回手牌。",
                    CardArtsId = "16110100",
                }
            },
            {
                "31003",//莫尔凡·符里斯
                new GwentCard()
                {
                    CardId ="31003",
                    EffectType=typeof(MorvranVoorhis),//效果
                    Name="莫尔凡·符里斯",
                    Strength=7,
                    Group=Group.Leader,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Officer},//需要添加
                    Flavor = "夏日阳光在阿尔巴河平静的水面上熠熠生辉——我印象中的尼弗迦德就是这样啊。",
                    Info = "揭示最多4张牌。",
                    CardArtsId = "16110200",
                }
            },
            {
                "31001",//约翰·卡尔维特
                new GwentCard()
                {
                    CardId ="31001",
                    EffectType=typeof(JanCalveit),//效果
                    Name="约翰·卡尔维特",
                    Strength=5,
                    Group=Group.Leader,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Officer},//需要添加
                    Flavor = "剑只是统治者的工具之一。",
                    Info = "检视牌组顶端3张牌，打出1张。",
                    CardArtsId = "16110300",
                }
            },
            {
                "31004",//篡位者
                new GwentCard()
                {
                    CardId ="31004",
                    EffectType=typeof(Usurper),//效果
                    Name="篡位者",
                    Strength=1,
                    Group=Group.Leader,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Officer},//需要添加
                    Flavor = "王权怎能单凭出身的贵贱来随便决定？",
                    Info = "间谍。不限阵营地创造1张领袖牌，使其获得2点增益。",
                    CardArtsId = "20158000",
                }
            },
            {
                "32002",//沙斯希乌斯
                new GwentCard()
                {
                    CardId ="32002",
                    EffectType=typeof(Xarthisius),//效果
                    Name="沙斯希乌斯",
                    Strength=13,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage},//需要添加
                    Flavor = "占卜术、水卜术、肠卜术、蜡卜术、蛋卜术、烬卜术、尿卜术、雷卜术……",
                    Info = "检视对方牌组，将其中1张牌置于底端。",
                    CardArtsId = "16210800",
                }
            },
            {
                "32003",//瓦提尔·德·李道克斯
                new GwentCard()
                {
                    CardId ="32003",
                    EffectType=typeof(VattierDeRideaux),//效果
                    Name="瓦提尔·德·李道克斯",
                    Strength=11,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer},//需要添加
                    Flavor = "精心策划的暗算能解决所有麻烦。",
                    Info = "揭示最多2张己方手牌，再随机揭示相同数量的对方卡牌",
                    CardArtsId = "16210300",
                }
            },
            {
                "32004",//史提芬·史凯伦
                new GwentCard()
                {
                    CardId ="32004",
                    EffectType=typeof(StefanSkellen),//效果
                    Name="史提芬·史凯伦",
                    Strength=10,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer},//需要添加
                    Flavor = "我在我们未来皇后的脸上留下了伤疤，这是我最引以为傲的成就。",
                    Info = "将牌组任意1张卡牌移至顶端。若它为非间谍单位，则使其获得5点增益。",
                    CardArtsId = "16210600",
                }
            },
            {
                "32005",//蒂博尔·艾格布拉杰
                new GwentCard()
                {
                    CardId ="32005",
                    EffectType=typeof(TiborEggebracht),//效果
                    Name="蒂博尔·艾格布拉杰",
                    Strength=10,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer},//需要添加
                    Flavor = "蒂博尔以狂热的忠诚而闻名。据说皇帝驾崩时，他深深地鞠躬致敬，几乎可以说贴在了地上。",
                    Info = "休战：获得15点增益，随后对方抽1张铜色牌并揭示它",
                    CardArtsId = "16210700",
                }
            },
            {
                "32006",//威戈佛特兹
                new GwentCard()
                {
                    CardId ="32006",
                    EffectType=typeof(Vilgefortz),//效果
                    Name="威戈佛特兹",
                    Strength=9,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage},//需要添加
                    Flavor = "我们都是他棋盘上的棋子，而这棋局的规则，我们一无所知。",
                    Info = "摧毁1个友军单位，随后从牌组顶端打出1张牌；或休战：摧毁1个敌军单位，随后对方抽1张铜色牌并揭示它。",
                    CardArtsId = "16210500",
                }
            },
            {
                "32007",//希拉德
                new GwentCard()
                {
                    CardId ="32007",
                    EffectType=typeof(Shilard),//效果
                    Name="希拉德",
                    Strength=9,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer},//需要添加
                    Flavor = "所谓外交官，就是用华丽的辞藻，来透露一些只言片语。",
                    Info = "休战：若双方牌组都有牌，从双方牌组各抽1张牌。保留1张，将另一张给予对方。",
                    CardArtsId = "20007100",
                }
            },
            {
                "32008",//门诺·库霍恩
                new GwentCard()
                {
                    CardId ="32008",
                    EffectType=typeof(MennoCoehoorn),//效果
                    Name="门诺·库霍恩",
                    Strength=8,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer},//需要添加
                    Flavor = "细心的侦察分队比训练有素的军团更管用。",
                    Info = "对1个敌军单位造成4点伤害。若它为间谍单位，则直接将其摧毁。",
                    CardArtsId = "16210200",
                }
            },
            {
                "32009",//雷欧·邦纳特
                new GwentCard()
                {
                    CardId ="32009",
                    EffectType=typeof(LeoBonhart),//效果
                    Name="雷欧·邦纳特",
                    Strength=7,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "他就坐在那里，死盯着我，一言不发。他的眼睛，好像……鱼眼，没有眉毛，没有睫毛……活脱脱两颗水球包裹的黑石头。一片死寂中，他就那样凝视着我，这却比被痛打我一顿更让我不寒而栗。",
                    Info = "揭示己方1张单位牌，对1个敌军单位造成等同于被揭示单位牌基础战力的伤害。",
                    CardArtsId = "20003100",
                }
            },
            {
                "32001",//亚特里的林法恩
                new GwentCard()
                {
                    CardId ="32001",
                    EffectType=typeof(RainfarnOfAttre),//效果
                    Name="亚特里的林法恩",
                    Strength=5,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer},//需要添加
                    Flavor = "辛特拉沦陷后，亚特里随即告破，这里的守军若不听任尼弗迦德人驱策，就只能去死。",
                    Info = "从牌组打出1张铜色/银色间谍单位牌。",
                    CardArtsId = "20003200",
                }
            },
            {
                "32010",//叶奈法：女术士
                new GwentCard()
                {
                    CardId ="32010",
                    EffectType=typeof(YenneferEnchantress),//效果
                    Name="叶奈法：女术士",
                    Strength=5,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Aedirn},//需要添加
                    Flavor = "最好不要挡叶奈法的道。特别是当她忙着赶路的时候。",
                    Info = "生成1张己方上张打出的铜色/银色“法术”牌。",
                    CardArtsId = "20160100",
                }
            },
            {
                "32011",//雷索：弑王者
                new GwentCard()
                {
                    CardId ="32011",
                    EffectType=typeof(LethoKingslayer),//效果
                    Name="雷索：弑王者",
                    Strength=5,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "这僧侣为什么戴着镶钉手套……？",
                    Info = "择一：摧毁1名敌军领袖，自身获得5点增益；或从牌组打出1张铜色/银色“谋略”牌。",
                    CardArtsId = "20160300",
                }
            },
            {
                "32012",//叶奈法：死灵法师
                new GwentCard()
                {
                    CardId ="32012",
                    EffectType=typeof(YenneferNecromancer),//效果
                    Name="叶奈法：死灵法师",
                    Strength=5,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Aedirn},//需要添加
                    Flavor = "身体已经开始腐烂……但声带依然完好。说不定我们还能让他开口说话……",
                    Info = "从对方墓场中复活1张铜色/银色“士兵”牌",
                    CardArtsId = "20178000",
                }
            },
            {
                "32013",//卡西尔·迪弗林
                new GwentCard()
                {
                    CardId ="32013",
                    EffectType=typeof(CahirDyffryn),//效果
                    Name="卡西尔·迪弗林",
                    Strength=1,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer,Categorie.Doomed},//需要添加
                    Flavor = "他的双眼在翼盔下熠熠生辉，剑刃上反射着火光。",
                    Info = "复活1张领袖牌。",
                    CardArtsId = "16210400",
                }
            },
            {
                "32014",//古雷特的雷索
                new GwentCard()
                {
                    CardId ="32014",
                    EffectType=typeof(LethoOfGulet),//效果
                    Name="古雷特的雷索",
                    Strength=1,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "猎魔人绝不会死在自己的床上。",
                    Info = "间谍。改变同排2个单位的锁定状态，随后汲食它们的所有战力。",
                    CardArtsId = "16210100",
                }
            },
            {
                "32015",//暗算
                new GwentCard()
                {
                    CardId ="32015",
                    EffectType=typeof(Assassination),//效果
                    Name="暗算",
                    Strength=0,
                    Group=Group.Gold,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special},//需要添加
                    Flavor = "“请你出手要多少钱？” “看情况喽。比如说目标是你，大改100奥伦币左右。”",
                    Info = "对1个敌军单位造成8点伤害，再对1个敌军单位造成8点伤害。",
                    CardArtsId = "16310100",
                }
            },
            {
                "33004",//坎塔蕾拉
                new GwentCard()
                {
                    CardId ="33004",
                    EffectType=typeof(Cantarella),//效果
                    Name="坎塔蕾拉",
                    Strength=13,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Agent},//需要添加
                    Flavor = "男人渴望着诱惑，神秘加优雅往往事半功倍。",
                    Info = "间谍、力竭。抽2张牌。保留1张，将另1张置于牌组底端。",
                    CardArtsId = "16221000",
                }
            },
            {
                "33005",//艾希蕾·阿纳兴
                new GwentCard()
                {
                    CardId ="33005",
                    EffectType=typeof(Assire),//效果
                    Name="艾希蕾·阿纳兴",
                    Strength=11,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage},//需要添加
                    Flavor = "尼弗迦德法师也有选择：要么服从，要么上断头台。",
                    Info = "将双方墓场中最多2张铜色/银色牌放回各自牌组。",
                    CardArtsId = "16220200",
                }
            },
            {
                "33003",//魔像守卫
                new GwentCard()
                {
                    CardId ="33003",
                    EffectType=typeof(TheGuardian),//效果
                    Name="魔像守卫",
                    Strength=11,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Construct},//需要添加
                    Flavor = "石拳阻挡刀剑，逻辑战胜谎言。",
                    Info = "将1个“次级魔像守卫”置于对方牌组顶端。",
                    CardArtsId = "16240100",
                }
            },
            {
                "33006",//亚伯力奇
                new GwentCard()
                {
                    CardId ="33006",
                    EffectType=typeof(Albrich),//效果
                    Name="亚伯力奇",
                    Strength=10,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage},//需要添加
                    Flavor = "火球？没问题。陛下想要什么都行。",
                    Info = "休战：双方各抽1张牌。对方抽到的牌将被揭示。",
                    CardArtsId = "16220100",
                }
            },
            {
                "33007",//斯维尔
                new GwentCard()
                {
                    CardId ="33007",
                    EffectType=typeof(Sweers),//效果
                    Name="斯维尔",
                    Strength=9,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer},//需要添加
                    Flavor = "别碰那女孩！听明白没，你们这群乡巴佬？",
                    Info = "选择1个敌军单位或对手手牌中1张被揭示的单位牌，将它所有的同名牌从其牌组置入其墓场。",
                    CardArtsId = "16220600",
                }
            },
            {
                "33008",//亨利·凡·阿特里
                new GwentCard()
                {
                    CardId ="33008",
                    EffectType=typeof(HenryVarAttre),//效果
                    Name="亨利·凡·阿特里",
                    Strength=9,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support},//需要添加
                    Flavor = "我在诺维格瑞驻守了十三年，什么我没见过？残忍、讥嘲、贪欲。可如今发生的事情却让我极度不安",
                    Info = "隐匿任意数量的单位。若为友军单位，则使它们获得2点增益；若为敌军单位，则对他们造成2点伤害。",
                    CardArtsId = "20022700",
                }
            },
            {
                "33002",//冒牌希里
                new GwentCard()
                {
                    CardId ="33002",
                    EffectType=typeof(FalseCiri),//效果
                    Name="冒牌希里",
                    Strength=8,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "“她来了，”他心想，“皇帝的联姻对象。冒牌公主、冒牌的辛特拉女王、冒牌的雅鲁加河口统治者，也是帝国未来的命脉。”",
                    Info = "间谍。回合开始时，若为间谍，则获得1点增益。对方放弃跟牌后，移至另一侧同排。遗愿：摧毁同排最弱的单位。",
                    CardArtsId = "16221200",
                }
            },
            {
                "33009",//重弩海尔格
                new GwentCard()
                {
                    CardId ="33009",
                    EffectType=typeof(HeftyHelge),//效果
                    Name="重弩海尔格",
                    Strength=8,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Machine},//需要添加
                    Flavor = "并非攻城的最佳选择，却是毁城的行家里手。",
                    Info = "对对方半场非同排上的所有敌军单位造成1点伤害。若被揭示，则对所有敌军单位造成1点伤害。",
                    CardArtsId = "20004100",
                }
            },
            {
                "33010",//奥克斯
                new GwentCard()
                {
                    CardId ="33010",
                    EffectType=typeof(Auckes),//效果
                    Name="奥克斯",
                    Strength=7,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "雷索已有计划……还能出什么岔子？",
                    Info = "改变2个单位的锁定状态。",
                    CardArtsId = "16220800",
                }
            },
            {
                "33011",//彼得·萨尔格温利
                new GwentCard()
                {
                    CardId ="33011",
                    EffectType=typeof(PeterSaarGwynleve),//效果
                    Name="彼得·萨尔格温利",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer},//需要添加
                    Flavor = "这双手不属于什么“阁下”，只属于一介农夫。所以这是农夫与农夫间的对话。",
                    Info = "重置1个友军单位，使其获得3点强化；或重置1个敌军单位，使其受到3点削弱。",
                    CardArtsId = "16220400",
                }
            },
            {
                "33012",//瑟瑞特
                new GwentCard()
                {
                    CardId ="33012",
                    EffectType=typeof(Serrit),//效果
                    Name="瑟瑞特",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "这是我们必须做的，我并不为此感到羞愧。",
                    Info = "对1个敌军单位造成7点伤害，或将对方1张被揭示的单位牌战力降为1点",
                    CardArtsId = "16220900",
                }
            },
            {
                "33013",//辛西亚
                new GwentCard()
                {
                    CardId ="33013",
                    EffectType=typeof(Cynthia),//效果
                    Name="辛西亚",
                    Strength=5,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage},//需要添加
                    Flavor = "决不能轻视辛西亚，必须把她看紧点儿。",
                    Info = "揭示对方手牌中最强的单位牌，并获得等同于其战力的增益。",
                    CardArtsId = "16220300",
                }
            },
            {
                "33001",//约阿希姆·德·维特
                new GwentCard()
                {
                    CardId ="33001",
                    EffectType=typeof(JoachimDeWett),//效果
                    Name="约阿希姆·德·维特",
                    Strength=5,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer},//需要添加
                    Flavor = "即便用“无能”来形容德·维特公爵对维登集团军的领导，都算给他面子",
                    Info = "间谍。从牌组顶端打出1张铜色/银色非间谍单位牌，并使它获得10点增益。",
                    CardArtsId = "16221100",
                }
            },
            {
                "33014",//维尔海夫
                new GwentCard()
                {
                    CardId ="33014",
                    EffectType=typeof(Vrygheff),//效果
                    Name="维尔海夫",
                    Strength=5,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer},//需要添加
                    Flavor = "以眼还眼，我们最后一定会全都变成瞎子。",
                    Info = "从牌组打出1张铜色“机械”牌。",
                    CardArtsId = "20136700",
                }
            },
            {
                "33015",//凡赫玛
                new GwentCard()
                {
                    CardId ="33015",
                    EffectType=typeof(Vanhemar),//效果
                    Name="凡赫玛",
                    Strength=4,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage},//需要添加
                    Flavor = "作为火法师，他算不上……多么热烈。",
                    Info = "生成“刺骨冰霜”、“晴空”或“复仇”。",
                    CardArtsId = "16220700",
                }
            },
            {
                "33016",//弗林姆德
                new GwentCard()
                {
                    CardId ="33016",
                    EffectType=typeof(Vreemde),//效果
                    Name="弗林姆德",
                    Strength=4,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer},//需要添加
                    Flavor = "即便要击败你们每一个人，我也要让伟大日轮照耀北境。",
                    Info = "创造1个铜色尼弗迦德“士兵”单位。",
                    CardArtsId = "20005000",
                }
            },
            {
                "33017",//契拉克·迪弗林
                new GwentCard()
                {
                    CardId ="33017",
                    EffectType=typeof(CeallachDyffryn),//效果
                    Name="契拉克·迪弗林",
                    Strength=2,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer},//需要添加
                    Flavor = "“陛下……”皇家总管呜咽着说。直到刚才为止，根本没人留意他。“求您发发慈悲……卡西尔……我的儿子……”",
                    Info = "生成1个“大使”、“刺客”或“特使”。",
                    CardArtsId = "16221300",
                }
            },
            {
                "33018",//芙琳吉拉·薇歌
                new GwentCard()
                {
                    CardId ="33018",
                    EffectType=typeof(FringillaVigo),//效果
                    Name="芙琳吉拉·薇歌",
                    Strength=1,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage},//需要添加
                    Flavor = "魔法的价值高于一切，高于所有争论和敌意。",
                    Info = "间谍。将左侧单位的战力复制给右侧单位。",
                    CardArtsId = "16220500",
                }
            },
            {
                "33019",//达兹伯格符文石
                new GwentCard()
                {
                    CardId ="33019",
                    EffectType=typeof(DazhbogRunestone),//效果
                    Name="达兹伯格符文石",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "当心。还烫着呢。",
                    Info = "创造1张铜色/银色“尼弗迦德”牌。",
                    CardArtsId = "20158300",
                }
            },
            {
                "33020",//通敌
                new GwentCard()
                {
                    CardId ="33020",
                    EffectType=typeof(Treason),//效果
                    Name="通敌",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special},//需要添加
                    Flavor = "许多人相信，帝国的实力在于纪律严明的军队与恪尽职守的法师。但事实上，金钱才是尼弗迦德统治世界的关键。",
                    Info = "迫使2个相邻敌军单位互相对决。",
                    CardArtsId = "16320100",
                }
            },
            {
                "33021",//吊死鬼之毒
                new GwentCard()
                {
                    CardId ="33021",
                    EffectType=typeof(Cadaverine),//效果
                    Name="吊死鬼之毒",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "我不会拿自己的性命去碰运气。我会拿上一把长剑，厚厚地涂上一层吊死鬼之毒。",
                    Info = "择一：对1个敌军单位以及所有与它同类型的单位造成2点伤害；或摧毁1个铜色/银色“中立”单位。",
                    CardArtsId = "20154000",
                }
            },
            {
                "33022",//尼弗迦德大门
                new GwentCard()
                {
                    CardId ="33022",
                    EffectType=typeof(NilfgaardianGate),//效果
                    Name="尼弗迦德大门",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special},//需要添加
                    Flavor = "应该不会对游客开放……",
                    Info = "从牌组打出1张铜色/银色“军官”牌，并使其获得1点增益。",
                    CardArtsId = "20055600",
                }
            },
            {
                "33023",//奴隶贩子
                new GwentCard()
                {
                    CardId ="33023",
                    EffectType=typeof(SlaveDriver),//效果
                    Name="奴隶贩子",
                    Strength=10,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer},//需要添加
                    Flavor = "奴隶贩子只有一个追求：那就是把他的命令一丝不苟地执行下去。",
                    Info = "将一个友军单位的战力降至1点，并对一个敌军单位造成伤害，数值等同于该友方单位所失去的战力。",
                    CardArtsId = "20137700",
                }
            },
            {
                "34004",//尼弗迦德骑士
                new GwentCard()
                {
                    CardId ="34004",
                    EffectType=typeof(NilfgaardianKnight),//效果
                    Name="尼弗迦德骑士",
                    Strength=12,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "出自名门望族，生于金塔之城，组成帝国的精锐部队。",
                    Info = "随机揭示1张己方手牌。2点护甲。",
                    CardArtsId = "16231800",
                }
            },
            {
                "34006",//伪装大师
                new GwentCard()
                {
                    CardId ="34006",
                    EffectType=typeof(MasterOfDisguise),//效果
                    Name="伪装大师",
                    Strength=11,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "哦，高悬上苍的伟大日轮啊，把我们从裤腿里除之不尽的虱子中解救出来吧！",
                    Info = "隐匿2张牌。",
                    CardArtsId = "20012000",
                }
            },
            {
                "34007",//阿尔巴师矛兵
                new GwentCard()
                {
                    CardId ="34007",
                    EffectType=typeof(AlbaSpearmen),//效果
                    Name="阿尔巴师矛兵",
                    Strength=10,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "为了帝国，至死不渝！",
                    Info = "任意方抽牌时获得1点增益。",
                    CardArtsId = "16231200",
                }
            },
            {
                "34008",//渗透者
                new GwentCard()
                {
                    CardId ="34008",
                    EffectType=typeof(Infiltrator),//效果
                    Name="渗透者",
                    Strength=10,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "你可以一直逃，但你永远藏不了。",
                    Info = "改变1个单位的间谍状态。",
                    CardArtsId = "20011800",
                }
            },
            {
                "34009",//炼金术士
                new GwentCard()
                {
                    CardId ="34009",
                    EffectType=typeof(Alchemist),//效果
                    Name="炼金术士",
                    Strength=9,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage},//需要添加
                    Flavor = "两盎司钙素，一盎司结合剂……",
                    Info = "揭示2张牌。",
                    CardArtsId = "16231600",
                }
            },
            {
                "34010",//军旗手
                new GwentCard()
                {
                    CardId ="34010",
                    EffectType=typeof(StandardBearer),//效果
                    Name="军旗手",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "“你要拼上性命，保卫这面军旗！Gloir aen Ard Feain！”",
                    Info = "己方每打出1张“士兵”单位牌，便使1个友军单位获得2点增益。",
                    CardArtsId = "20029400",
                }
            },
            {
                "34011",//投尸机
                new GwentCard()
                {
                    CardId ="34011",
                    EffectType=typeof(RotTosser),//效果
                    Name="投尸机",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Machine},//需要添加
                    Flavor = "向围城中散播瘟疫是否人道，这个话题还是留给史学家吧。我们只关心这法子有没有效。",
                    Info = "在对方单排生成1个“牛尸”。",
                    CardArtsId = "16230200",
                }
            },
            {
                "34012",//奴隶猎人
                new GwentCard()
                {
                    CardId ="34012",
                    EffectType=typeof(SlaveHunter),//效果
                    Name="奴隶猎人",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "诀窍在于摧残他们的意志，而不是肉体",
                    Info = "魅惑1个战力不高于3点的铜色敌军单位。",
                    CardArtsId = "20133700",
                }
            },
            {
                "34013",//哨卫
                new GwentCard()
                {
                    CardId ="34013",
                    EffectType=typeof(Sentry),//效果
                    Name="哨卫",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "要造桥，不要造城墙。",
                    Info = "使1个“士兵”单位的所有同名牌获得2点增益。",
                    CardArtsId = "20166100",
                }
            },
            {
                "34014",//阿尔巴师装甲骑兵
                new GwentCard()
                {
                    CardId ="34014",
                    EffectType=typeof(AlbaArmoredCavalry),//效果
                    Name="阿尔巴师装甲骑兵",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "他们以雷动之势冲进敌军方阵，犹如一把尖刀插入柔软的肚腹。阿尔巴之师所向披靡，一路横扫，直取泰莫利亚步兵团的咽喉。",
                    Info = "每有1个友军单位被打出，便获得1点增益。",
                    CardArtsId = "20029600",
                }
            },
            {
                "34015",//戴斯文强弩手
                new GwentCard()
                {
                    CardId ="34015",
                    EffectType=typeof(DeithwenArbalest),//效果
                    Name="戴斯文强弩手",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "我只瞄膝盖，一向如此。",
                    Info = "对1个敌军单位造成3点伤害。若它为间谍，则伤害提升至6点。",
                    CardArtsId = "16230500",
                }
            },
            {
                "34016",//射石机
                new GwentCard()
                {
                    CardId ="34016",
                    EffectType=typeof(Mangonel),//效果
                    Name="射石机",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Machine},//需要添加
                    Flavor = "适用于投掷残骸和干粪。",
                    Info = "对1个敌军随机单位造成2点伤害。己方每揭示1张牌，便再次触发此能力。",
                    CardArtsId = "16231700",
                }
            },
            {
                "34017",//那乌西卡旅中士
                new GwentCard()
                {
                    CardId ="34017",
                    EffectType=typeof(NauzicaaSergeant),//效果
                    Name="那乌西卡旅中士",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Officer},//需要添加
                    Flavor = "皇帝会教北方人懂点儿规矩的。",
                    Info = "移除所在排的灾厄效果，并使1个友军单位或1个手牌中被揭示的单位获得3点增益。",
                    CardArtsId = "16230900",
                }
            },
            {
                "34018",//作战工程师
                new GwentCard()
                {
                    CardId ="34018",
                    EffectType=typeof(CombatEngineer),//效果
                    Name="作战工程师",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support},//需要添加
                    Flavor = "战争是文明进步的确凿证据——看看吧，现在大伙儿打仗更有效率了。",
                    Info = "使1个友军单位获得5点增益。\n操控。",
                    CardArtsId = "16231300",
                }
            },
            {
                "34003",//近卫军
                new GwentCard()
                {
                    CardId ="34003",
                    EffectType=typeof(ImperaBrigade),//效果
                    Name="近卫军",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "近卫军绝不投降，绝不。",
                    Info = "场上每有1个敌军间谍单位，便获得2点增益。\n每有1个间谍单位出现在对方半场，便获得2点增益。",
                    CardArtsId = "16230700",
                }
            },
            {
                "34005",//近卫军铁卫
                new GwentCard()
                {
                    CardId ="34005",
                    EffectType=typeof(ImperaEnforcers),//效果
                    Name="近卫军铁卫",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "皇帝最忠诚的贴身护卫，誓死保卫皇帝的安全。",
                    Info = "对1名敌军单位造成2点伤害。\n己方回合内每出现1个敌军间谍单位，便在回合结束时对1个敌军单位造成2点单位。",
                    CardArtsId = "16230800",
                }
            },
            {
                "34019",//火蝎攻城弩
                new GwentCard()
                {
                    CardId ="34019",
                    EffectType=typeof(FireScorpion),//效果
                    Name="火蝎攻城弩",
                    Strength=5,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Machine},//需要添加
                    Flavor = "名字与实物可谓风马牛不相及，听上去它好像是某种肥大的红色甲壳生物，但其实是一种由能工巧匠精心设计的大规模杀伤性武器……",
                    Info = "对1个敌军单位造成5点伤害。\n被己方揭示时，再次触发此能力。",
                    CardArtsId = "16230600",
                }
            },
            {
                "34020",//那乌西卡旅
                new GwentCard()
                {
                    CardId ="34020",
                    EffectType=typeof(NauzicaaBrigade),//效果
                    Name="那乌西卡旅",
                    Strength=5,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "我们被称作死神头。想知道为什么吗？",
                    Info = "对1个间谍单位造成7点伤害。若摧毁目标，则获得4点强化。",
                    CardArtsId = "16231000",
                }
            },
            {
                "34021",//侦察员
                new GwentCard()
                {
                    CardId ="34021",
                    EffectType=typeof(Spotter),//效果
                    Name="侦察员",
                    Strength=5,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "北方佬耍不出花招了。",
                    Info = "获得等同于1张被揭示铜色/银色单位牌基础战力的增益。",
                    CardArtsId = "16230300",
                }
            },
            {
                "34022",//毒蛇学派猎魔人
                new GwentCard()
                {
                    CardId ="34022",
                    EffectType=typeof(ViperWitcher),//效果
                    Name="毒蛇学派猎魔人",
                    Strength=5,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Witcher},//需要添加
                    Flavor = "毒蛇学派将会重生……雷索志在必得。",
                    Info = "己方起始牌组中每有1张“炼金”牌，便造成1点伤害。",
                    CardArtsId = "20012400",
                }
            },
            {
                "34023",//迪尔兰士兵
                new GwentCard()
                {
                    CardId ="34023",
                    EffectType=typeof(DaerlanSoldier),//效果
                    Name="迪尔兰士兵",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "我在布莱班特军事学院学到了不少东西，比如洗土豆",
                    Info = "被己方揭示时直接打出至随机排，然后抽1张牌。",
                    CardArtsId = "16230100",
                }
            },
            {
                "34024",//阿尔巴师枪兵
                new GwentCard()
                {
                    CardId ="34024",
                    EffectType=typeof(AlbaPikeman),//效果
                    Name="阿尔巴师枪兵",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "宣誓效忠吾皇恩希尔·恩瑞斯……不然就受刑吧！",
                    Info = "召唤所有同名牌。",
                    CardArtsId = "16231100",
                }
            },
            {
                "34025",//帝国魔像
                new GwentCard()
                {
                    CardId ="34025",
                    EffectType=typeof(ImperialGolem),//效果
                    Name="帝国魔像",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Construct},//需要添加
                    Flavor = "尼弗迦德最强大的法师们终于成功掌握了制造魔像的方法。更棒的是，他们已经“偶尔”能让这些魔像为帝国而战了……",
                    Info = "每当己方揭示1张对方手牌，便从牌组召唤1张同名牌。",
                    CardArtsId = "13240700",
                }
            },
            {
                "34026",//马格尼师
                new GwentCard()
                {
                    CardId ="34026",
                    EffectType=typeof(MagneDivision),//效果
                    Name="马格尼师",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "皇帝固然应该胸怀天下……但派人守卫这么一块不毛之地，实在是浪费资源。",
                    Info = "从牌组随机打出1张铜色“道具”牌。",
                    CardArtsId = "20004400",
                }
            },
            {
                "34027",//奴隶步兵
                new GwentCard()
                {
                    CardId ="34027",
                    EffectType=typeof(SlaveInfantry),//效果
                    Name="奴隶步兵",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "自由人可以选择。奴隶只能由别人替他们决定。",
                    Info = "在己方其他排生成1张佚亡原始同名牌。",
                    CardArtsId = "20136500",
                }
            },
            {
                "34028",//大使
                new GwentCard()
                {
                    CardId ="34028",
                    EffectType=typeof(Ambassador),//效果
                    Name="大使",
                    Strength=2,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "间谍？不，这么说就太过啦。我觉得自己不过是个观察员而已。",
                    Info = "间谍。使1个友军单位获得12点增益。",
                    CardArtsId = "16231500",
                }
            },
            {
                "34002",//特使
                new GwentCard()
                {
                    CardId ="34002",
                    EffectType=typeof(Emissary),//效果
                    Name="特使",
                    Strength=2,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "但是……这样不对啊！两国交兵不斩来使！",
                    Info = "间谍。随机检视牌组中2张铜色单位牌，打出1张。",
                    CardArtsId = "16231400",
                }
            },
            {
                "34001",//维可瓦罗见习法师
                new GwentCard()
                {
                    CardId ="34001",
                    EffectType=typeof(VicovaroNovice),//效果
                    Name="维可瓦罗见习法师",
                    Strength=2,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage},//需要添加
                    Flavor = "尼弗迦德的法师们就像手纸。一旦谁被皇帝厌倦，立刻有大把新人迫不及待地赶来补缺。",
                    Info = "检视牌组中2张铜色“炼金”牌，打出1张。",
                    CardArtsId = "12240300",
                }
            },
            {
                "34029",//刺客
                new GwentCard()
                {
                    CardId ="34029",
                    EffectType=typeof(Assassin),//效果
                    Name="刺客",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "帝国宫廷最受欢迎的职业之一，仅次于抄书吏和轻浮的贵妇。",
                    Info = "间谍对左侧单位造成10点伤害。",
                    CardArtsId = "20011500",
                }
            },
            {
                "34030",//维可瓦罗医师
                new GwentCard()
                {
                    CardId ="34030",
                    EffectType=typeof(VicovaroMedic),//效果
                    Name="维可瓦罗医师",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support,Categorie.Doomed},//需要添加
                    Flavor = "这个世界的瘟疫跟战争一样多，夺人性命也一样地出其不意。",
                    Info = "从对方墓场复活1个铜色单位。",
                    CardArtsId = "16230400",
                }
            },
            {
                "34031",//文登达尔精锐
                new GwentCard()
                {
                    CardId ="34031",
                    EffectType=typeof(VenendalElite),//效果
                    Name="文登达尔精锐",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "艾宾以其顶尖的雇佣兵和轻骑兵闻名天下。",
                    Info = "与1个被揭示的单位互换战力。",
                    CardArtsId = "20051800",
                }
            },
            {
                "34032",//新兵
                new GwentCard()
                {
                    CardId ="34032",
                    EffectType=typeof(Recruit),//效果
                    Name="新兵",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "他也不问为什么，只能……一个接一个地削那该死的马铃薯。",
                    Info = "从牌组随机打出1张非同名“铜色”士兵牌。",
                    CardArtsId = "20161700",
                }
            },
            {
                "34033",//油膏
                new GwentCard()
                {
                    CardId ="34033",
                    EffectType=typeof(Ointment),//效果
                    Name="油膏",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "它管用吗？谁知道呢。反正试试也没坏处。可能吧。",
                    Info = "复活1个战力不高于5点的铜色单位。",
                    CardArtsId = "20153900",
                }
            },
            {
                "35001",//次级魔像守卫
                new GwentCard()
                {
                    CardId ="35001",
                    EffectType=typeof(NoneEffect),//效果
                    Name="次级魔像守卫",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Construct,Categorie.Token},//需要添加
                    Flavor = "按理说死去的守卫者不该再坚守岗位，但魔法可往往不会遵循常理……",
                    Info = "没有特殊技能。",
                    CardArtsId = "16240100",
                }
            },
            {
                "35002",//牛尸
                new GwentCard()
                {
                    CardId ="35002",
                    EffectType=typeof(CowCarcass),//效果
                    Name="牛尸",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.Nilfgaard,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = true,
                    Countdown = 2,//冷却2
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Token},//需要添加
                    Flavor = "那是只鸟？还是头狮鹫？不对！那是……",
                    Info = "间谍。2回合后，己方回合结束时，摧毁同排所有其他最弱的单位，并放逐自身。",
                    CardArtsId = "16240200",
                }
            },
            {
                "41001",//拉多维德五世
                new GwentCard()
                {
                    CardId ="41001",
                    EffectType=typeof(KingRadovidV),//效果
                    Name="拉多维德五世",
                    Strength=6,
                    Group=Group.Leader,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Redania},//需要添加
                    Flavor = "身为国王，对待敌人应冷酷无情，对待朋友当慷慨大度。",
                    Info = "改变2个单位的锁定状态。若为敌军单位，则对其造成4点伤害。 操控。",
                    CardArtsId = "12110200",
                }
            },
            {
                "41002",//雅妲公主
                new GwentCard()
                {
                    CardId ="41002",
                    EffectType=typeof(PrincessAdda),//效果
                    Name="雅妲公主",
                    Strength=6,
                    Group=Group.Leader,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Cursed},//需要添加
                    Flavor = "她的诅咒被解除了……但是喜欢吃生肉的习惯却没改掉。",
                    Info = "创造1个铜色/银色“诅咒生物”单位。",
                    CardArtsId = "20006300",
                }
            },
            {
                "41003",//弗尔泰斯特国王
                new GwentCard()
                {
                    CardId ="41003",
                    EffectType=typeof(KingFoltest),//效果
                    Name="弗尔泰斯特国王",
                    Strength=5,
                    Group=Group.Leader,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Temeria},//需要添加
                    Flavor = "我不需要谋臣和他们的诡计，我相信士兵的利刃。",
                    Info = "使己方半场其他单位，以及手牌和牌组中的非间谍单位获得1点增益。 操控。",
                    CardArtsId = "12110100",
                }
            },
            {
                "41004",//亨赛特国王
                new GwentCard()
                {
                    CardId ="41004",
                    EffectType=typeof(KingHenselt),//效果
                    Name="亨赛特国王",
                    Strength=3,
                    Group=Group.Leader,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Kaedwen},//需要添加
                    Flavor = "恕我直言，亨赛特国王不是看起来像贼，他本就是贼。",
                    Info = "选择1个友军铜色“机械”或“科德温”单位，从牌组打出所有它的同名牌。 操控。",
                    CardArtsId = "12110300",
                }
            },
            {
                "42001",//丹德里恩
                new GwentCard()
                {
                    CardId ="42001",
                    EffectType=typeof(Dandelion),//效果
                    Name="丹德里恩",
                    Strength=11,
                    Group=Group.Gold,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support},//需要添加
                    Flavor = "丹德里恩，你愤世嫉俗、好色成性又谎话连篇——却也是我最好的朋友。",
                    Info = "使牌组3个单位获得2点增益。",
                    CardArtsId = "12220100",
                }
            },
            {
                "42002",//血红男爵
                new GwentCard()
                {
                    CardId ="42002",
                    EffectType=typeof(BloodyBaron),//效果
                    Name="血红男爵",
                    Strength=9,
                    Group=Group.Gold,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Temeria,Categorie.Officer},//需要添加
                    Flavor = "我知道自己不是个好父亲，但……现在弥补或许还来得及。",
                    Info = "若位于手牌、牌组或己方半场：有敌军单位被摧毁时获得1点增益。",
                    CardArtsId = "12210100",
                }
            },
            {
                "42003",//古雷特的赛尔奇克
                new GwentCard()
                {
                    CardId ="42003",
                    EffectType=typeof(SeltkirkOfGulet),//效果
                    Name="古雷特的赛尔奇克",
                    Strength=8,
                    Group=Group.Gold,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Aedirn,Categorie.Officer},//需要添加
                    Flavor = "尽管赛尔奇克美德过人，更是有着不屈的勇气，然而他还是和所有命丧上亚甸之战的将士一样，没能逃过自己的命运。",
                    Info = "与1个敌军单位对决。 3点护甲。",
                    CardArtsId = "20161800",
                }
            },
            {
                "42004",//范德格里夫特
                new GwentCard()
                {
                    CardId ="42004",
                    EffectType=typeof(Vandergrift),//效果
                    Name="范德格里夫特",
                    Strength=7,
                    Group=Group.Gold,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Kaedwen,Categorie.Officer},//需要添加
                    Flavor = "将军本以为洛马克的战争会很快结束，不会有什么损失……结果却陷入了永恒的征战。",
                    Info = "对所有敌军单位造成1点伤害。在被摧毁的单位同排降下“终末之战”。",
                    CardArtsId = "20162000",
                }
            },
            {
                "42005",//约翰·纳塔利斯
                new GwentCard()
                {
                    CardId ="42005",
                    EffectType=typeof(JohnNatalis),//效果
                    Name="约翰·纳塔利斯",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Temeria,Categorie.Officer},//需要添加
                    Flavor = "那座广场应以我的士兵及其他牺牲者来命名，而不是顶着我的名字。",
                    Info = "从牌组打出1张铜色/银色“谋略”牌。",
                    CardArtsId = "12210300",
                }
            },
            {
                "42006",//凯拉·梅兹
                new GwentCard()
                {
                    CardId ="42006",
                    EffectType=typeof(KeiraMetz),//效果
                    Name="凯拉·梅兹",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Temeria},//需要添加
                    Flavor = "即便今天死，我也要死得光鲜亮丽。",
                    Info = "生成“阿尔祖落雷术”、“雷霆”或“蟹蜘蛛毒液”。",
                    CardArtsId = "12210800",
                }
            },
            {
                "42007",//罗契：冷酷之心
                new GwentCard()
                {
                    CardId ="42007",
                    EffectType=typeof(RocheMerciless),//效果
                    Name="罗契：冷酷之心",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Temeria,Categorie.Officer},//需要添加
                    Flavor = "我们内心从不畏惧。不过，倒是有一个人类……弗农·罗契。千万要当心他。",
                    Info = "摧毁1个背面向上的伏击敌军单位。",
                    CardArtsId = "20177700",
                }
            },
            {
                "42008",//迪杰斯特拉
                new GwentCard()
                {
                    CardId ="42008",
                    EffectType=typeof(SigismundDijkstra),//效果
                    Name="迪杰斯特拉",
                    Strength=4,
                    Group=Group.Gold,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Redania},//需要添加
                    Flavor = "你真以为这种下三滥的谎话能糊弄我？",
                    Info = "间谍。 从牌组随机打出2张牌。",
                    CardArtsId = "12210500",
                }
            },
            {
                "42009",//夏妮
                new GwentCard()
                {
                    CardId ="42009",
                    EffectType=typeof(Shani),//效果
                    Name="夏妮",
                    Strength=4,
                    Group=Group.Gold,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Redania,Categorie.Support,Categorie.Doomed},//需要添加
                    Flavor = "我是医师，不会乱开药。",
                    Info = "复活1个铜色/银色非“诅咒生物”单位，并使其获得2点护甲。",
                    CardArtsId = "12210600",
                }
            },
            {
                "42010",//凯亚恩
                new GwentCard()
                {
                    CardId ="42010",
                    EffectType=typeof(Kiyan),//效果
                    Name="凯亚恩",
                    Strength=4,
                    Group=Group.Gold,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Witcher},//需要添加
                    Flavor = "我们生活在无尽黑色大海中一座宁静的世外小岛上。我们不该扬帆远航。",
                    Info = "择一：创造1张铜色/银色“炼金”牌；或从牌组打出1张铜色/银色“道具”牌。",
                    CardArtsId = "20162100",
                }
            },
            {
                "42011",//普西拉
                new GwentCard()
                {
                    CardId ="42011",
                    EffectType=typeof(Priscilla),//效果
                    Name="普西拉",
                    Strength=3,
                    Group=Group.Gold,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support},//需要添加
                    Flavor = "想像一下穿裙子的丹德里恩，大概就是那副模样。",
                    Info = "随机使5个友军单位获得3点增益。",
                    CardArtsId = "12220200",
                }
            },
            {
                "42012",//弗农·罗契
                new GwentCard()
                {
                    CardId ="42012",
                    EffectType=typeof(VernonRoche),//效果
                    Name="弗农·罗契",
                    Strength=3,
                    Group=Group.Gold,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Temeria,Categorie.Officer},//需要添加
                    Flavor = "一位爱国者……也是个令人头疼的家伙。",
                    Info = "对1个敌军单位造成7点伤害。 对局开始时，将1个“蓝衣铁卫突击队”加入牌组。",
                    CardArtsId = "12210200",
                }
            },
            {
                "42013",//菲丽芭·艾哈特
                new GwentCard()
                {
                    CardId ="42013",
                    EffectType=typeof(PhilippaEilhart),//效果
                    Name="菲丽芭·艾哈特",
                    Strength=1,
                    Group=Group.Gold,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Redania},//需要添加
                    Flavor = "王族的权势即将凋零，女术士集会所必将崛起。",
                    Info = "对敌军单位造成5、4、3、2、1点伤害。每次随机改变目标，无法对同一目标连续造成伤害。",
                    CardArtsId = "12210400",
                }
            },
            {
                "43001",//塔勒
                new GwentCard()
                {
                    CardId ="43001",
                    EffectType=typeof(Thaler),//效果
                    Name="塔勒",
                    Strength=13,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Temeria,Categorie.Agent},//需要添加
                    Flavor = "滚开！我们中间不是每个人都那么轻浮、那么浅薄……",
                    Info = "间谍、力竭。 抽2张牌，保留1张，放回另1张。",
                    CardArtsId = "12220300",
                }
            },
            {
                "43002",//薇丝
                new GwentCard()
                {
                    CardId ="43002",
                    EffectType=typeof(Ves),//效果
                    Name="薇丝",
                    Strength=12,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Temeria},//需要添加
                    Flavor = "宁似帝王快活一天，强如乞丐苟活一世。",
                    Info = "交换最多2张牌。",
                    CardArtsId = "12220400",
                }
            },
            {
                "43003",//巨魔魔
                new GwentCard()
                {
                    CardId ="43003",
                    EffectType=typeof(Trollololo),//效果
                    Name="巨魔魔",
                    Strength=11,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Redania,Categorie.Ogroid},//需要添加
                    Flavor = "我是拉多多德国王的兵兵。收到命令，看守船船。",
                    Info = "9点护甲。",
                    CardArtsId = "12220900",
                }
            },
            {
                "43004",//没完没了的朗维德
                new GwentCard()
                {
                    CardId ="43004",
                    EffectType=typeof(RonvidTheIncessant),//效果
                    Name="没完没了的朗维德",
                    Strength=11,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Kaedwen},//需要添加
                    Flavor = "这位自封的骑士四海漂泊，捍卫着心爱少女比贝莉的荣誉。虽然没人知道她住在哪里，她是不是哈活着，甚至是不是曾经有过这个人。",
                    Info = "回合结束时，复活至随机排，基础战力设为1点。 操控。",
                    CardArtsId = "20046500",
                }
            },
            {
                "43005",//异婴
                new GwentCard()
                {
                    CardId ="43005",
                    EffectType=typeof(Botchling),//效果
                    Name="异婴",
                    Strength=10,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed},//需要添加
                    Flavor = "承认错误，妥善安葬——否则他们会回来缠着你不放。",
                    Info = "召唤1只“家事妖精”。",
                    CardArtsId = "12240100",
                }
            },
            {
                "43006",//南尼克
                new GwentCard()
                {
                    CardId ="43006",
                    EffectType=typeof(Nenneke),//效果
                    Name="南尼克",
                    Strength=10,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support,Categorie.Temeria},//需要添加
                    Flavor = "南尼克的医术无人可及。",
                    Info = "将墓场3张铜色/银色单位牌放回牌组。",
                    CardArtsId = "12221200",
                }
            },
            {
                "43007",//弗尔泰斯特之傲
                new GwentCard()
                {
                    CardId ="43007",
                    EffectType=typeof(FoltestSPride),//效果
                    Name="弗尔泰斯特之傲",
                    Strength=10,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Machine},//需要添加
                    Flavor = "正如弗尔泰斯特国王常说的那样，尺寸并不重要，关键要好用。",
                    Info = "对1个敌军单位造成2点伤害，并将其上移一排。 驱动：再次触发此能力。",
                    CardArtsId = "20149400",
                }
            },
            {
                "43008",//文森特·梅斯
                new GwentCard()
                {
                    CardId ="43008",
                    EffectType=typeof(VincentMeis),//效果
                    Name="文森特·梅斯",
                    Strength=9,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Cursed},//需要添加
                    Flavor = "白天是维吉玛城卫兵队长。到了晚上，便化身为无情的复仇者，保卫受尽压迫的劳苦大众。",
                    Info = "摧毁所有单位的护甲，获得被摧毁护甲数值一半的增益。",
                    CardArtsId = "20009800",
                }
            },
            {
                "43009",//欧德林
                new GwentCard()
                {
                    CardId ="43009",
                    EffectType=typeof(Odrin),//效果
                    Name="欧德林",
                    Strength=8,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Kaedwen},//需要添加
                    Flavor = "喝酒少了欧德林，就像划船没带桨。",
                    Info = "回合开始时，移至随机排，并使同排所有友军单位获得1点增益。",
                    CardArtsId = "12221300",
                }
            },
            {
                "43010",//休伯特·雷亚克
                new GwentCard()
                {
                    CardId ="43010",
                    EffectType=typeof(HubertRejk),//效果
                    Name="休伯特·雷亚克",
                    Strength=7,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Vampire},//需要添加
                    Flavor = "验尸官是个冷静、淡定的人。就算是尸体，也会精心照料。",
                    Info = "汲食牌组中所有单位的增益，作为战力。",
                    CardArtsId = "20008800",
                }
            },
            {
                "43011",//玛格丽塔
                new GwentCard()
                {
                    CardId ="43011",
                    EffectType=typeof(MargaritaOfAretuza),//效果
                    Name="玛格丽塔",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Temeria},//需要添加
                    Flavor = "我只关心艾瑞图萨学院和我的学生们。",
                    Info = "重置1个单位，并改变它的锁定状态。",
                    CardArtsId = "12221100",
                }
            },
            {
                "43012",//家事妖精
                new GwentCard()
                {
                    CardId ="43012",
                    EffectType=typeof(Lubberkin),//效果
                    Name="家事妖精",
                    Strength=5,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed},//需要添加
                    Flavor = "吾为汝取名蒂雅，收汝为吾女。",
                    Info = "召唤1只“异婴”。",
                    CardArtsId = "12240200",
                }
            },
            {
                "43013",//戴斯摩
                new GwentCard()
                {
                    CardId ="43013",
                    EffectType=typeof(Dethmold),//效果
                    Name="戴斯摩",
                    Strength=4,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Kaedwen},//需要添加
                    Flavor = "我曾让一个囚犯搜肠刮肚地狂吐……啊，好怀念……",
                    Info = "生成“倾盆大雨”、“晴空”或“阿尔祖落雷术”。",
                    CardArtsId = "12220700",
                }
            },
            {
                "43014",//席儿·德·坦沙维耶
                new GwentCard()
                {
                    CardId ="43014",
                    EffectType=typeof(SLeDeTansarville),//效果
                    Name="席儿·德·坦沙维耶",
                    Strength=4,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage},//需要添加
                    Flavor = "女术士集会所缺乏谦逊，对权力的渴望可能会让我们功亏一篑。",
                    Info = "从手牌打出1张铜色/银色“特殊”牌，随后抽1张牌。",
                    CardArtsId = "12220500",
                }
            },
            {
                "43015",//帕薇塔公主
                new GwentCard()
                {
                    CardId ="43015",
                    EffectType=typeof(PrincessPavetta),//效果
                    Name="帕薇塔公主",
                    Strength=3,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Cintra},//需要添加
                    Flavor = "他们说公主容易动怒，但我没想到……",
                    Info = "将双方最弱的铜色/银色单位放回各自牌组。",
                    CardArtsId = "12221000",
                }
            },
            {
                "43016",//斯坦尼斯王子
                new GwentCard()
                {
                    CardId ="43016",
                    EffectType=typeof(PrinceStennis),//效果
                    Name="斯坦尼斯王子",
                    Strength=3,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Aedirn,Categorie.Officer},//需要添加
                    Flavor = "他穿着黄金铠甲——没错，混账一个。",
                    Info = "从牌组随机打出1张铜色/银色非间谍单位牌，使其获得5点护甲。 3点护甲。",
                    CardArtsId = "12220800",
                }
            },
            {
                "43017",//萨宾娜·葛丽维希格
                new GwentCard()
                {
                    CardId ="43017",
                    EffectType=typeof(SabrinaGlevissig),//效果
                    Name="萨宾娜·葛丽维希格",
                    Strength=3,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Kaedwen},//需要添加
                    Flavor = "科德温荒野之女。",
                    Info = "间谍。 遗愿：将该排最弱单位的战力应用于全排单位。",
                    CardArtsId = "12220600",
                }
            },
            {
                "43018",//萨宾娜的幽灵
                new GwentCard()
                {
                    CardId ="43018",
                    EffectType=typeof(SabrinaSSpecter),//效果
                    Name="萨宾娜的幽灵",
                    Strength=3,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Cursed,Categorie.Doomed},//需要添加
                    Flavor = "萨宾娜·葛丽维希格用尽最后一口气，释放出一道强大的诅咒，不但处刑人遭了殃，留在附近的所有人也都没能幸免。",
                    Info = "复活1个铜色“诅咒生物”单位。",
                    CardArtsId = "20165000",
                }
            },
            {
                "43019",//佐里亚符文石
                new GwentCard()
                {
                    CardId ="43019",
                    EffectType=typeof(ZoriaRunestone),//效果
                    Name="佐里亚符文石",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "这块符文石让我不寒而栗……我是不是做了什么伤它感情的事？",
                    Info = "创造1张铜色/银色“北方领域”牌。",
                    CardArtsId = "20158200",
                }
            },
            {
                "43020",//增援
                new GwentCard()
                {
                    CardId ="43020",
                    EffectType=typeof(Reinforcement),//效果
                    Name="增援",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special},//需要添加
                    Flavor = "吹号撤退！重新集结！等待增援！",
                    Info = "从牌组打出1张铜色/银色“士兵”、“机械”、“军官”或“辅助”单位牌。",
                    CardArtsId = "12320100",
                }
            },
            {
                "43021",//范德格里夫特之剑
                new GwentCard()
                {
                    CardId ="43021",
                    EffectType=typeof(VandergriftSBlade),//效果
                    Name="范德格里夫特之剑",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "在阿德卡莱的一次骑士比武中，赛尔奇克打断了范德格里夫特的长剑。于是，愤怒的范德格里夫特下令铸造一把新的兵刃，还在上面附了强大的符文石。",
                    Info = "择一：摧毁1个铜色/银色“诅咒单位”敌军单位；或造成9点伤害，放逐所摧毁的单位。",
                    CardArtsId = "20163300",
                }
            },
            {
                "44001",//崔丹姆步兵
                new GwentCard()
                {
                    CardId ="44001",
                    EffectType=typeof(TridamInfantry),//效果
                    Name="崔丹姆步兵",
                    Strength=10,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "他们本是忠于崔丹姆老男爵的士兵，随法利波离开城市后，如今却成了被悬赏的叛徒。",
                    Info = "4点护甲。",
                    CardArtsId = "20017100",
                }
            },
            {
                "44002",//班·阿德导师
                new GwentCard()
                {
                    CardId ="44002",
                    EffectType=typeof(BanArdTutor),//效果
                    Name="班·阿德导师",
                    Strength=9,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Kaedwen},//需要添加
                    Flavor = "我一直拿艾瑞图萨学院的女生和班·阿德的愣头小子们作对比。结果总是姑娘们胜出。",
                    Info = "用1张手牌交换牌组中的一张铜色“特殊”牌。",
                    CardArtsId = "20004800",
                }
            },
            {
                "44003",//科德温中士
                new GwentCard()
                {
                    CardId ="44003",
                    EffectType=typeof(KaedweniSergeant),//效果
                    Name="科德温中士",
                    Strength=9,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Kaedwen,Categorie.Officer},//需要添加
                    Flavor = "冲锋，没用的蠢货！冲锋，不然你就会知道，我比尼弗迦德人更可怕！",
                    Info = "移除所在排的灾厄。 3点护甲。 操控。",
                    CardArtsId = "12221400",
                }
            },
            {
                "44004",//科德温骑兵
                new GwentCard()
                {
                    CardId ="44004",
                    EffectType=typeof(KaedweniCavalry),//效果
                    Name="科德温骑兵",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Kaedwen},//需要添加
                    Flavor = "我一直很好奇，这帮家伙是怎么解决内急的？",
                    Info = "摧毁1个单位的护甲。扣除的护甲值将被转化为自身增益。",
                    CardArtsId = "12231400",
                }
            },
            {
                "44005",//战地医师
                new GwentCard()
                {
                    CardId ="44005",
                    EffectType=typeof(FieldMedic),//效果
                    Name="战地医师",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support},//需要添加
                    Flavor = "红缝红，白缝白，活儿真不赖。",
                    Info = "使友军“士兵”单位获得1点增益。",
                    CardArtsId = "12231200",
                }
            },
            {
                "44006",//瑞达尼亚精锐
                new GwentCard()
                {
                    CardId ="44006",
                    EffectType=typeof(RedanianElite),//效果
                    Name="瑞达尼亚精锐",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Redania,Categorie.Soldier},//需要添加
                    Flavor = "为了瑞达尼亚，不论上刀山下油锅，还是吃虫子，我都在所不惜。",
                    Info = "每当护甲归0，获得5点增益。 4点护甲。",
                    CardArtsId = "12231700",
                }
            },
            {
                "44007",//攻城塔
                new GwentCard()
                {
                    CardId ="44007",
                    EffectType=typeof(SiegeTower),//效果
                    Name="攻城塔",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Machine},//需要添加
                    Flavor = "攻城用的最新武器。",
                    Info = "获得2点增益。 驱动：再次触发此能力。",
                    CardArtsId = "12230400",
                }
            },
            {
                "44008",//加强型投石机
                new GwentCard()
                {
                    CardId ="44008",
                    EffectType=typeof(ReinforcedTrebuchet),//效果
                    Name="加强型投石机",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Machine},//需要添加
                    Flavor = "感受到了吗？每当这宝贝儿投出巨石，大地都会震颤。",
                    Info = "回合结束时，对1个敌军随机单位造成1点伤害。",
                    CardArtsId = "12231500",
                }
            },
            {
                "44009",//科德温骑士
                new GwentCard()
                {
                    CardId ="44009",
                    EffectType=typeof(KaedweniKnight),//效果
                    Name="科德温骑士",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Kaedwen},//需要添加
                    Flavor = "科德温军队里并非所有人都认同国王的政治手段。然而谁都不敢说出来。",
                    Info = "若从牌组打出，则获得5点增益。 2点护甲。",
                    CardArtsId = "20003400",
                }
            },
            {
                "44010",//被诅咒的骑士
                new GwentCard()
                {
                    CardId ="44010",
                    EffectType=typeof(CursedKnight),//效果
                    Name="被诅咒的骑士",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Aedirn},//需要添加
                    Flavor = "深陷无尽的战斗，却早已忘记了起因。",
                    Info = "将1个友军“诅咒生物”单位变为自身的原始同名牌。 2点护甲。",
                    CardArtsId = "20162500",
                }
            },
            {
                "44011",//攻城后援
                new GwentCard()
                {
                    CardId ="44011",
                    EffectType=typeof(SiegeSupport),//效果
                    Name="攻城后援",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Kaedwen,Categorie.Support},//需要添加
                    Flavor = "“你得把准星左校5度。”“把什么调多少？”",
                    Info = "使后续打出的友军单位获得1点增益，“机械”单位额外获得1点护甲。 操控。",
                    CardArtsId = "12230900",
                }
            },
            {
                "44012",//瑞达尼亚骑士
                new GwentCard()
                {
                    CardId ="44012",
                    EffectType=typeof(RedanianKnight),//效果
                    Name="瑞达尼亚骑士",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Redania,Categorie.Soldier},//需要添加
                    Flavor = "为了荣耀，为了拉多维德陛下！",
                    Info = "回合结束时，若不受护甲保护，则获得2点增益和2点护甲。",
                    CardArtsId = "12230800",
                }
            },
            {
                "44013",//瑞达尼亚当选骑士
                new GwentCard()
                {
                    CardId ="44013",
                    EffectType=typeof(RedanianKnightElect),//效果
                    Name="瑞达尼亚当选骑士",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Redania,Categorie.Soldier},//需要添加
                    Flavor = "不为名不为利，高尚的动机才能成就英雄！",
                    Info = "回合结束时，若受护甲保护，则使相邻单位获得1点增益。 2点护甲。",
                    CardArtsId = "12330100",
                }
            },
            {
                "44014",//加强型弩炮
                new GwentCard()
                {
                    CardId ="44014",
                    EffectType=typeof(ReinforcedBallista),//效果
                    Name="加强型弩炮",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Machine},//需要添加
                    Flavor = "从没两次命中同一处，仔细想想，真是个大问题。",
                    Info = "对1个敌军单位造成2点伤害。 驱动：再次触发此能力。",
                    CardArtsId = "12230200",
                }
            },
            {
                "44015",//投石机
                new GwentCard()
                {
                    CardId ="44015",
                    EffectType=typeof(Trebuchet),//效果
                    Name="投石机",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Machine},//需要添加
                    Flavor = "城堡可是块硬骨头。快去准备投石机！",
                    Info = "对3个相邻敌军单位造成1点伤害。 驱动：伤害增加1点。",
                    CardArtsId = "12230300",
                }
            },
            {
                "44016",//亚甸槌击者
                new GwentCard()
                {
                    CardId ="44016",
                    EffectType=typeof(AedirnianMauler),//效果
                    Name="亚甸槌击者",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Aedirn},//需要添加
                    Flavor = "呃，这些家伙真让人头疼。",
                    Info = "对1个敌军造成4点伤害。",
                    CardArtsId = "20167500",
                }
            },
            {
                "44017",//弩炮
                new GwentCard()
                {
                    CardId ="44017",
                    EffectType=typeof(Ballista),//效果
                    Name="弩炮",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Machine},//需要添加
                    Flavor = "成为弩炮是所有十字弩的终极梦想。",
                    Info = "对1个敌军单位和最多4个与它战力相同的其他敌军单位造成1点伤害。 驱动：再次触发此能力。",
                    CardArtsId = "12230100",
                }
            },
            {
                "44018",//攻城槌
                new GwentCard()
                {
                    CardId ="44018",
                    EffectType=typeof(BatteringRam),//效果
                    Name="攻城槌",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Machine},//需要添加
                    Flavor = "不肯开门是吧？那我们就再加把劲敲一敲啰。",
                    Info = "对1个敌军单位造成3点伤害。若摧毁目标，则对另一个敌军单位造成3点伤害。 驱动：初始伤害提高1点。",
                    CardArtsId = "20004900",
                }
            },
            {
                "44019",//攻城大师
                new GwentCard()
                {
                    CardId ="44019",
                    EffectType=typeof(SiegeMaster),//效果
                    Name="攻城大师",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Kaedwen,Categorie.Support},//需要添加
                    Flavor = "我绝不会失手两次。",
                    Info = "治愈一个铜色/银色友军“机械”单位，并再次触发其能力。 操控。",
                    CardArtsId = "12231800",
                }
            },
            {
                "44020",//可怜的步兵
                new GwentCard()
                {
                    CardId ="44020",
                    EffectType=typeof(PoorFIngInfantry),//效果
                    Name="可怜的步兵",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Temeria},//需要添加
                    Flavor = "看在我是老兵的份上！打赏个一克朗吧？",
                    Info = "在左右两侧分别生成“左侧翼步兵”和“右侧翼步兵”。",
                    CardArtsId = "12230510",
                }
            },
            {
                "44021",//掠夺者猎人
                new GwentCard()
                {
                    CardId ="44021",
                    EffectType=typeof(ReaverHunter),//效果
                    Name="掠夺者猎人",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Redania,Categorie.Soldier},//需要添加
                    Flavor = "看见这纹身没？纹的是我在痛扁一条龙。一条龙哟，女士。",
                    Info = "使手牌、牌组或己方半场所有同名牌获得1点增益。 每有1张同名牌打出，便再次触发此能力。",
                    CardArtsId = "12230610",
                }
            },
            {
                "44022",//泰莫利亚鼓手
                new GwentCard()
                {
                    CardId ="44022",
                    EffectType=typeof(TemerianDrummer),//效果
                    Name="泰莫利亚鼓手",
                    Strength=5,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support,Categorie.Temeria},//需要添加
                    Flavor = "他告诉妈妈，自己想当个音乐家。只是没料到演奏的是这种乐器。",
                    Info = "使1个友军单位获得6点增益。",
                    CardArtsId = "20029900",
                }
            },
            {
                "44023",//褐旗营
                new GwentCard()
                {
                    CardId ="44023",
                    EffectType=typeof(DunBanner),//效果
                    Name="褐旗营",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Kaedwen},//需要添加
                    Flavor = "冷静，所有人保持警惕！这可能是那帮披斗篷戴海狸皮帽的家伙的陷阱……",
                    Info = "回合开始时，若落后25点战力以上，则召唤此单位至随机排。",
                    CardArtsId = "12231300",
                }
            },
            {
                "44024",//科德温亡魂
                new GwentCard()
                {
                    CardId ="44024",
                    EffectType=typeof(KaedweniRevenant),//效果
                    Name="科德温亡魂",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Kaedwen},//需要添加
                    Flavor = "万劫不复的士兵日复一日地过着同一天，就好像奇安凡尼银行的职员一样。",
                    Info = "己方打出下一张“法术”或“道具”牌时，在所在排生成1张自身的佚亡原始同名牌。 1点护甲。",
                    CardArtsId = "20162400",
                }
            },
            {
                "44025",//中邪的女术士
                new GwentCard()
                {
                    CardId ="44025",
                    EffectType=typeof(DamnedSorceress),//效果
                    Name="中邪的女术士",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Cursed},//需要添加
                    Flavor = "萨宾娜的诅咒谁也不放过，就连其他的女术士也难以幸免。",
                    Info = "若同排有1个“诅咒生物”单位，则造成7点伤害。",
                    CardArtsId = "20163000",
                }
            },
            {
                "44026",//艾瑞图萨学院学员
                new GwentCard()
                {
                    CardId ="44026",
                    EffectType=typeof(AretuzaAdept),//效果
                    Name="艾瑞图萨学院学员",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Temeria},//需要添加
                    Flavor = "这些女学员在艾瑞图萨活得像公主一样，任何怪诞的想法都能得到满足，同时半个城市都在服务于她们：裁缝、帽商、糖果商、贩夫走卒……",
                    Info = "从牌组随机打出1张铜色灾厄牌。",
                    CardArtsId = "20003300",
                }
            },
            {
                "44027",//蓝衣铁卫突击队
                new GwentCard()
                {
                    CardId ="44027",
                    EffectType=typeof(BlueStripesCommando),//效果
                    Name="蓝衣铁卫突击队",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Temeria},//需要添加
                    Flavor = "我愿为泰莫利亚赴汤蹈火，不过大多时候是在排除异己。",
                    Info = "有战力与自身相同的非同名“泰莫利亚”友军单位被打出时，从牌组召唤1张它的同名牌。",
                    CardArtsId = "12231110",
                }
            },
            {
                "44028",//蓝衣铁卫斥候
                new GwentCard()
                {
                    CardId ="44028",
                    EffectType=typeof(BlueStripesScout),//效果
                    Name="蓝衣铁卫斥候",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Temeria},//需要添加
                    Flavor = "蓝衣铁卫与松鼠党有个共通点——满心仇恨。",
                    Info = "使己方半场其他“泰莫利亚”单位，以及手牌和牌组所有战力与自身相同的非间谍“泰莫利亚”单位获得1点增益。 操控。",
                    CardArtsId = "12231000",
                }
            },
            {
                "44029",//泰莫利亚步兵
                new GwentCard()
                {
                    CardId ="44029",
                    EffectType=typeof(TemerianInfantryman),//效果
                    Name="泰莫利亚步兵",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Temeria},//需要添加
                    Flavor = "泰莫利亚！泰莫利亚！诸神恩赐汝等！降怒于敌，使其永世灾厄！",
                    Info = "召唤所有同名牌。",
                    CardArtsId = "12231610",
                }
            },
            {
                "44030",//女巫猎人
                new GwentCard()
                {
                    CardId ="44030",
                    EffectType=typeof(WitchHunter),//效果
                    Name="女巫猎人",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Redania,Categorie.Soldier},//需要添加
                    Flavor = "女巫猎人都是些什么人？大部分都是流氓无赖、狂热分子。然而看到女术士集会所犯下的肮脏罪行，不少怒不可遏的体面人也加入了他们的行列……",
                    Info = "重置1个单位。若它为“法师”，则从牌组打出1张同名牌。",
                    CardArtsId = "20013200",
                }
            },
            {
                "44031",//受折磨的法师
                new GwentCard()
                {
                    CardId ="44031",
                    EffectType=typeof(TormentedMage),//效果
                    Name="受折磨的法师",
                    Strength=2,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Cursed},//需要添加
                    Flavor = "萨宾娜·葛丽维希格的诅咒摧枯拉朽。面对这股怒火，就连法师们也束手无策。",
                    Info = "检视牌组中2张铜色“法术”/“道具”牌，打出1张。",
                    CardArtsId = "20162800",
                }
            },
            {
                "44032",//掠夺者斥候
                new GwentCard()
                {
                    CardId ="44032",
                    EffectType=typeof(ReaverScout),//效果
                    Name="掠夺者斥候",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Redania,Categorie.Support},//需要添加
                    Flavor = "最近没怪兽可杀，所以我们从军了。",
                    Info = "选择1个非同名友军铜色单位，从牌组打出1张它的同名牌。",
                    CardArtsId = "12230700",
                }
            },
            {
                "44033",//绞盘
                new GwentCard()
                {
                    CardId ="44033",
                    EffectType=typeof(Winch),//效果
                    Name="绞盘",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Tactic,Categorie.Special},//需要添加
                    Flavor = "就是一个绞盘。没什么可大惊小怪的。",
                    Info = "使所有己方半场的“机械”单位获得3点增益。",
                    CardArtsId = "20165900",
                }
            },
            {
                "44034",//染血连枷
                new GwentCard()
                {
                    CardId ="44034",
                    EffectType=typeof(BloodyFlail),//效果
                    Name="染血连枷",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "有些武器在整个北方领域都禁止使用。因为它们所造成的伤害超出了人们的想象。",
                    Info = "造成5点伤害，并在随机排生成1只“鬼灵”。",
                    CardArtsId = "20150300",
                }
            },
            {
                "45001",//鬼灵
                new GwentCard()
                {
                    CardId ="45001",
                    EffectType=typeof(NoneEffect),//效果
                    Name="鬼灵",
                    Strength=5,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Token},//需要添加
                    Flavor = "身陷无尽的战斗，却早已忘记了起因。",
                    Info = "没有特殊技能。",
                    CardArtsId = "20045700",
                }
            },
            {
                "45002",//左侧翼步兵
                new GwentCard()
                {
                    CardId ="45002",
                    EffectType=typeof(NoneEffect),//效果
                    Name="左侧翼步兵",
                    Strength=2,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Temeria,Categorie.Token},//需要添加
                    Flavor = "看在我是老兵的份上！打赏个一克朗吧？",
                    Info = "没有特殊技能。",
                    CardArtsId = "12230500",
                }
            },
            {
                "45003",//右侧翼步兵
                new GwentCard()
                {
                    CardId ="45003",
                    EffectType=typeof(NoneEffect),//效果
                    Name="右侧翼步兵",
                    Strength=2,
                    Group=Group.Copper,
                    Faction = Faction.NorthernRealms,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Temeria,Categorie.Token},//需要添加
                    Flavor = "看在我是老兵的份上！打赏个一克朗吧？",
                    Info = "没有特殊技能。",
                    CardArtsId = "12230520",
                }
            },
            {
                "51001",//法兰茜丝卡
                new GwentCard()
                {
                    CardId ="51001",
                    EffectType=typeof(FrancescaFindabair),//效果
                    Name="法兰茜丝卡",
                    Strength=7,
                    Group=Group.Leader,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Mage,Categorie.Elf},//需要添加
                    Flavor = "世上没有轻而易举的和平，人类的压迫终将画下残酷的句点。",
                    Info = "选择1张牌进行交换，交换所得的卡牌获得3点增益。",
                    CardArtsId = "14110100",
                }
            },
            {
                "51002",//艾思娜
                new GwentCard()
                {
                    CardId ="51002",
                    EffectType=typeof(Eithn),//效果
                    Name="艾思娜",
                    Strength=5,
                    Group=Group.Leader,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Dryad},//需要添加
                    Flavor = "树精女王眼似融银，心如冷钢。",
                    Info = "复活1张铜色/银色“特殊”牌。",
                    CardArtsId = "14110200",
                }
            },
            {
                "51003",//菲拉凡德芮
                new GwentCard()
                {
                    CardId ="51003",
                    EffectType=typeof(Filavandrel),//效果
                    Name="菲拉凡德芮",
                    Strength=4,
                    Group=Group.Leader,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Elf},//需要添加
                    Flavor = "虽然我们人数不多、四散分离，但我们的内心燃烧得比任何时候都要炽热。",
                    Info = "创造1张银色“特殊”牌。",
                    CardArtsId = "20007500",
                }
            },
            {
                "51004",//布罗瓦尔·霍格
                new GwentCard()
                {
                    CardId ="51004",
                    EffectType=typeof(BrouverHoog),//效果
                    Name="布罗瓦尔·霍格",
                    Strength=4,
                    Group=Group.Leader,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.Dwarf},//需要添加
                    Flavor = "那个老糊涂？你甚至分不清他是活人还是木偶！",
                    Info = "从牌组打出1张非间谍银色单位牌或铜色“矮人”牌。",
                    CardArtsId = "14110300",
                }
            },
            {
                "52001",//萨琪亚
                new GwentCard()
                {
                    CardId ="52001",
                    EffectType=typeof(Saskia),//效果
                    Name="萨琪亚",
                    Strength=11,
                    Group=Group.Gold,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Aedirn,Categorie.Draconid},//需要添加
                    Flavor = "王权于我毫无意义，只有东方那位才配为尊。",
                    Info = "用最多2张牌交换同等数量的铜色牌。",
                    CardArtsId = "20020900",
                }
            },
            {
                "52002",//萨琪亚萨司
                new GwentCard()
                {
                    CardId ="52002",
                    EffectType=typeof(Saesenthessis),//效果
                    Name="萨琪亚萨司",
                    Strength=10,
                    Group=Group.Gold,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Aedirn,Categorie.Draconid},//需要添加
                    Flavor = "我继承了父亲的变身能力……好吧，尽管我只有一种变化形态。",
                    Info = "增益自身等同于友军“矮人”单位数量；造成等同于友军“精灵”单位数量的伤害。",
                    CardArtsId = "14210100",
                }
            },
            {
                "52003",//泽维尔·莫兰
                new GwentCard()
                {
                    CardId ="52003",
                    EffectType=typeof(XavierMoran),//效果
                    Name="泽维尔·莫兰",
                    Strength=10,
                    Group=Group.Gold,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Dwarf},//需要添加
                    Flavor = "怎么了，公主？吃不惯野味吗？嗯？",
                    Info = "增益自身等同于最后打出的非同名“矮人”单位牌的初始战力。",
                    CardArtsId = "20008000",
                }
            },
            {
                "52004",//艾格莱丝
                new GwentCard()
                {
                    CardId ="52004",
                    EffectType=typeof(AglaS),//效果
                    Name="艾格莱丝",
                    Strength=8,
                    Group=Group.Gold,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Dryad},//需要添加
                    Flavor = "布洛克莱昂在滴血……可就连我也无能为力。",
                    Info = "从对方墓场复活1张铜色/银色“特殊”牌，随后将其放逐。",
                    CardArtsId = "14210600",
                }
            },
            {
                "52005",//卓尔坦·齐瓦
                new GwentCard()
                {
                    CardId ="52005",
                    EffectType=typeof(ZoltanChivay),//效果
                    Name="卓尔坦·齐瓦",
                    Strength=8,
                    Group=Group.Gold,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Dwarf},//需要添加
                    Flavor = "一人单独喝酒的滋味，就好比俩人一起蹲坑。",
                    Info = "选择3个单位，将它们移至所在半场的此排。使其中的友军单位获得2点强化；对其中的敌军单位造成2点伤害。",
                    CardArtsId = "14210500",
                }
            },
            {
                "52006",//伊森格林
                new GwentCard()
                {
                    CardId ="52006",
                    EffectType=typeof(IsengrimFaoiltiarna),//效果
                    Name="伊森格林",
                    Strength=7,
                    Group=Group.Gold,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Elf,Categorie.Officer},//需要添加
                    Flavor = "他们一见我的疤就知道：这下死定了。",
                    Info = "从牌组打出1张铜色/银色伏击牌。",
                    CardArtsId = "14210200",
                }
            },
            {
                "52007",//伊欧菲斯
                new GwentCard()
                {
                    CardId ="52007",
                    EffectType=typeof(Iorveth),//效果
                    Name="伊欧菲斯",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Elf,Categorie.Officer},//需要添加
                    Flavor = "国王还是乞丐于我并无差别，人类少一个算一个。",
                    Info = "对1个敌军单位造成8点伤害。若目标被摧毁，则使手牌中所有“精灵”单位获得1点增益。",
                    CardArtsId = "14210300",
                }
            },
            {
                "52008",//米尔瓦
                new GwentCard()
                {
                    CardId ="52008",
                    EffectType=typeof(Milva),//效果
                    Name="米尔瓦",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "每次拉弓射箭，我总想起父亲。他……应该会为我骄傲吧。",
                    Info = "将双方最强的铜色/银色单位收回各自牌组。",
                    CardArtsId = "14210400",
                }
            },
            {
                "52009",//莫丽恩：森林之女
                new GwentCard()
                {
                    CardId ="52009",
                    EffectType=typeof(MorennForestChild),//效果
                    Name="莫丽恩：森林之女",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Dryad},//需要添加
                    Flavor = "布洛克莱昂的意义远高于我的生命。她是一位母亲，关怀着自己的孩子们。我至死都要捍卫她。",
                    Info = "伏击：当对方打出下张铜色/银色特殊牌时，翻开并抵消其能力。",
                    CardArtsId = "20177900",
                }
            },
            {
                "52010",//希鲁
                new GwentCard()
                {
                    CardId ="52010",
                    EffectType=typeof(Schirr),//效果
                    Name="希鲁",
                    Strength=4,
                    Group=Group.Gold,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "直面死神的时候到了。",
                    Info = "生成“烧灼”或“瘟疫”。",
                    CardArtsId = "14210800",
                }
            },
            {
                "52011",//伊斯琳妮
                new GwentCard()
                {
                    CardId ="52011",
                    EffectType=typeof(IthlinneAegli),//效果
                    Name="伊斯琳妮",
                    Strength=2,
                    Group=Group.Gold,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Elf},//需要添加
                    Flavor = "她因不断预言世界末日而闻名遐迩——那些可不是什么好玩言论。",
                    Info = "从牌组打出1张铜色“法术”、恩泽或灾厄牌，重复其效果一次。",
                    CardArtsId = "14210700",
                }
            },
            {
                "52012",//伊欧菲斯：冥想
                new GwentCard()
                {
                    CardId ="52012",
                    EffectType=typeof(IorvethMeditation),//效果
                    Name="伊欧菲斯：冥想",
                    Strength=2,
                    Group=Group.Gold,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Elf,Categorie.Officer},//需要添加
                    Flavor = "即使伊欧菲斯只剩一只眼睛，他内心的洞察力也无人能及。",
                    Info = "迫使2个同排的敌军单位互相对决。",
                    CardArtsId = "20161100",
                }
            },
            {
                "52013",//伊森格林：亡命徒
                new GwentCard()
                {
                    CardId ="52013",
                    EffectType=typeof(IsengrimOutlaw),//效果
                    Name="伊森格林：亡命徒",
                    Strength=2,
                    Group=Group.Gold,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Elf,Categorie.Officer},//需要添加
                    Flavor = "在我们眼前的便是艾尔斯克德格山道，再往前，就是瑟瑞卡尼亚和哈克兰。这将是一条漫长而危险的道路。要想一同走下去，我们就得摒除彼此的猜忌。",
                    Info = "择一：从牌组打出1张铜色/银色“特殊”牌；或创造1个银色“精灵”单位。",
                    CardArtsId = "20161500",
                }
            },
            {
                "53001",//亚伊文
                new GwentCard()
                {
                    CardId ="53001",
                    EffectType=typeof(Yaevinn),//效果
                    Name="亚伊文",
                    Strength=13,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Elf,Categorie.Agent},//需要添加
                    Flavor = "我们是汇成风暴的雨滴。",
                    Info = "间谍、力竭。 抽1张“特殊”牌和单位牌。保留1张，放回另一张。",
                    CardArtsId = "14220300",
                }
            },
            {
                "53002",//艾雷亚斯
                new GwentCard()
                {
                    CardId ="53002",
                    EffectType=typeof(EleYas),//效果
                    Name="艾雷亚斯",
                    Strength=10,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "只要出于爱，疯狂就是合理的。",
                    Info = "被抽到或被收回牌组时获得2点增益。",
                    CardArtsId = "14221400",
                }
            },
            {
                "53003",//席朗·依斯尼兰
                new GwentCard()
                {
                    CardId ="53003",
                    EffectType=typeof(CiaranAepEasnillen),//效果
                    Name="席朗·依斯尼兰",
                    Strength=9,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "通往自由的路由鲜血铺就，而非墨水写成。",
                    Info = "改变1个单位的锁定状态，并把它移至其所在半场的同排。",
                    CardArtsId = "14220600",
                }
            },
            {
                "53004",//谢尔顿·斯卡格斯
                new GwentCard()
                {
                    CardId ="53004",
                    EffectType=typeof(SheldonSkaggs),//效果
                    Name="谢尔顿·斯卡格斯",
                    Strength=9,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Dwarf,Categorie.Officer},//需要添加
                    Flavor = "我可是在战况最激烈的前线！",
                    Info = "将同排所有友军单位移至随机排。每移动1个单位，便获得1点增益。",
                    CardArtsId = "14221200",
                }
            },
            {
                "53005",//丹尼斯·克莱默
                new GwentCard()
                {
                    CardId ="53005",
                    EffectType=typeof(DennisCranmer),//效果
                    Name="丹尼斯·克莱默",
                    Strength=8,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Dwarf,Categorie.Officer},//需要添加
                    Flavor = "我知道如何执行命令，冲别人去说教吧。",
                    Info = "使位于手牌、牌组和己方半场除自身外所有“矮人”单位获得1点强化。",
                    CardArtsId = "14220100",
                }
            },
            {
                "53006",//莫丽恩
                new GwentCard()
                {
                    CardId ="53006",
                    EffectType=typeof(Morenn),//效果
                    Name="莫丽恩",
                    Strength=8,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Dryad},//需要添加
                    Flavor = "艾思娜女士的女儿继承了她无与伦比的美貌，也同样极端仇视与人类有关的一切。",
                    Info = "伏击：在下个单位从任意方手牌打出至对方半场时翻开，对它造成7点伤害。",
                    CardArtsId = "14220800",
                }
            },
            {
                "53007",//亚尔潘·齐格林
                new GwentCard()
                {
                    CardId ="53007",
                    EffectType=typeof(YarpenZigrin),//效果
                    Name="亚尔潘·齐格林",
                    Strength=8,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Dwarf},//需要添加
                    Flavor = "听说过巨龙奥克维斯塔吗？石英山那只？亚尔潘·齐格林与他的矮人同伴们把它解决了。",
                    Info = "坚韧。 每打出1个友军“矮人”单位，便获得1点增益。",
                    CardArtsId = "14221300",
                }
            },
            {
                "53008",//玛丽娜
                new GwentCard()
                {
                    CardId ="53008",
                    EffectType=typeof(Malena),//效果
                    Name="玛丽娜",
                    Strength=7,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Elf},//需要添加
                    Flavor = "我恨你们，人类。你们全都一个样。",
                    Info = "伏击：2回合后的回合开始时：翻开，在战力不超过5点的铜色/银色敌军单位中魅惑其中最强的一个。",
                    CardArtsId = "14221000",
                }
            },
            {
                "53009",//爱黎瑞恩
                new GwentCard()
                {
                    CardId ="53009",
                    EffectType=typeof(Aelirenn),//效果
                    Name="爱黎瑞恩",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Elf,Categorie.Officer},//需要添加
                    Flavor = "苟活不如好死。",
                    Info = "场上有至少5个“精灵”友军单位时，在回合结束时召唤此单位。",
                    CardArtsId = "14221100",
                }
            },
            {
                "53010",//布蕾恩
                new GwentCard()
                {
                    CardId ="53010",
                    EffectType=typeof(Braenn),//效果
                    Name="布蕾恩",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Dryad},//需要添加
                    Flavor = "莫娜？不，不。我是布蕾恩。布洛克莱昂的女儿。",
                    Info = "对1个敌军单位造成等同于自身战力的伤害。若目标被摧毁，则使位于手牌、牌组和己方半场除自身外所有“树精”和 伏击单位获得1点增益。",
                    CardArtsId = "14220900",
                }
            },
            {
                "53011",//托露薇尔
                new GwentCard()
                {
                    CardId ="53011",
                    EffectType=typeof(Toruviel),//效果
                    Name="托露薇尔",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Elf,Categorie.Officer},//需要添加
                    Flavor = "我很乐意站在你面前，直视你的双眼然后干掉你……但你臭死了，人类。",
                    Info = "伏击：对方放弃跟牌后翻开，使左右各2格内的单位获得2点增益。",
                    CardArtsId = "14220400",
                }
            },
            {
                "53012",//帕扶科“舅舅”盖尔
                new GwentCard()
                {
                    CardId ="53012",
                    EffectType=typeof(PavkoGale),//效果
                    Name="帕扶科“舅舅”盖尔",
                    Strength=5,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier},//需要添加
                    Flavor = "为蓝山的精灵走私粮食最多的人当属盖尔。他为他们带来了一袋袋芜菁，还有最最珍贵的韭葱，因此被亲切地称为“舅舅”。",
                    Info = "从牌组打出1张铜色/银色“道具”牌。",
                    CardArtsId = "20167600",
                }
            },
            {
                "53013",//艾达·艾敏
                new GwentCard()
                {
                    CardId ="53013",
                    EffectType=typeof(IdaEmeanAepSivney),//效果
                    Name="艾达·艾敏",
                    Strength=4,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Elf},//需要添加
                    Flavor = "我是名贤者。我的力量源于占有知识，而非传播知识。",
                    Info = "生成“蔽日浓雾”、“晴空”或“阿尔祖落雷术”。",
                    CardArtsId = "14220200",
                }
            },
            {
                "53014",//麦莉
                new GwentCard()
                {
                    CardId ="53014",
                    EffectType=typeof(Milaen),//效果
                    Name="麦莉",
                    Strength=4,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Elf},//需要添加
                    Flavor = "老男爵对待偷猎者向来毫不留情。幸亏麦莉运气够好，老男爵已经死了，他的手下也成了亡命之徒。",
                    Info = "选定一排，做左右两侧末端的单位各造成6点伤害。",
                    CardArtsId = "6010300",
                }
            },
            {
                "53015",//哈托利
                new GwentCard()
                {
                    CardId ="53015",
                    EffectType=typeof(IbhearHattori),//效果
                    Name="哈托利",
                    Strength=3,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Elf,Categorie.Support,Categorie.Doomed},//需要添加
                    Flavor = "只有一样东西能和他打造的长剑相媲美——他包的饺子。",
                    Info = "复活1个战力不高于自身的铜色/银色“松鼠党”单位。",
                    CardArtsId = "20052000",
                }
            },
            {
                "53016",//保利·达尔伯格
                new GwentCard()
                {
                    CardId ="53016",
                    EffectType=typeof(PaulieDahlberg),//效果
                    Name="保利·达尔伯格",
                    Strength=3,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support,Categorie.Dwarf,Categorie.Doomed},//需要添加
                    Flavor = "快走！这是个该死的陷阱！",
                    Info = "复活一个铜色非“辅助”矮人单位。",
                    CardArtsId = "20169600",
                }
            },
            {
                "53017",//巴克莱·艾尔斯
                new GwentCard()
                {
                    CardId ="53017",
                    EffectType=typeof(BarclayEls),//效果
                    Name="巴克莱·艾尔斯",
                    Strength=2,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Dwarf,Categorie.Officer},//需要添加
                    Flavor = "嫌我们的蜂蜜酒味道不好？好办，堵住你的鼻子就行了！",
                    Info = "从牌组打出1张随机铜色/银色矮人牌，并使其获得3点强化。",
                    CardArtsId = "14220700",
                }
            },
            {
                "53018",//莫拉纳符文石
                new GwentCard()
                {
                    CardId ="53018",
                    EffectType=typeof(MoranaRunestone),//效果
                    Name="莫拉纳符文石",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "一看到它我就头晕……",
                    Info = "创造1张铜色/银色“松鼠党”牌。",
                    CardArtsId = "20158500",
                }
            },
            {
                "53019",//自然的馈赠
                new GwentCard()
                {
                    CardId ="53019",
                    EffectType=typeof(NatureSGift),//效果
                    Name="自然的馈赠",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special},//需要添加
                    Flavor = "你们贪得无厌地榨干大地，野蛮强横地攫取它的财富。但在我们这儿，它生机勃勃，春华秋实，慷慨大方。因为它爱我们，正如我们爱它。",
                    Info = "从牌组打出1张铜色/银色“特殊”牌。",
                    CardArtsId = "14320100",
                }
            },
            {
                "53020",//陷坑
                new GwentCard()
                {
                    CardId ="53020",
                    EffectType=typeof(PitTrap),//效果
                    Name="陷坑",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "简单、廉价，又十分好用。难怪它是松鼠党最喜欢用的一种陷阱。",
                    Info = "在对方单排降下灾厄，对所有被影响的单位造成3点伤害。",
                    CardArtsId = "20149000",
                }
            },
            {
                "53021",//玛哈坎号角
                new GwentCard()
                {
                    CardId ="53021",
                    EffectType=typeof(MahakamHorn),//效果
                    Name="玛哈坎号角",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "从前，玛哈坎举办过一次吹号角比赛。那一天，矮人们学到了重要的一课：不要在积雪殷厚的雪上下大声吹号。",
                    Info = "择一：创造1张铜色/银色“矮人”牌；或使1个单位获得7点强化。",
                    CardArtsId = "20153700",
                }
            },
            {
                "54001",//维里赫德旅工兵
                new GwentCard()
                {
                    CardId ="54001",
                    EffectType=typeof(VriheddSappers),//效果
                    Name="维里赫德旅工兵",
                    Strength=11,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "不管流言怎么说，精灵才不会碰人类的头皮。因为虱子太多了。",
                    Info = "伏击：2回合后，在回合开始时翻开。",
                    CardArtsId = "14230700",
                }
            },
            {
                "54002",//精灵斥候
                new GwentCard()
                {
                    CardId ="54002",
                    EffectType=typeof(ElvenScout),//效果
                    Name="精灵斥候",
                    Strength=10,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "他们说精灵踏雪无痕。不过要我说，“他们”不过是一帮住在乡下的白痴，就知道胡说八道。",
                    Info = "交换一张牌。",
                    CardArtsId = "20143800",
                }
            },
            {
                "54003",//维里赫德旅新兵
                new GwentCard()
                {
                    CardId ="54003",
                    EffectType=typeof(VriheddNeophyte),//效果
                    Name="维里赫德旅新兵",
                    Strength=10,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "许多非人种族在城市里饱受歧视和排斥，于是决定加入松鼠党。",
                    Info = "随机使手牌中2个单位获得1点增益。",
                    CardArtsId = "14240100",
                }
            },
            {
                "54004",//维里赫德旅
                new GwentCard()
                {
                    CardId ="54004",
                    EffectType=typeof(VriheddBrigade),//效果
                    Name="维里赫德旅",
                    Strength=9,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "“维里赫德旅？那是什么？”“麻烦。”",
                    Info = "移除所在排的灾厄，并将1个单位移至它所在半场的同排。",
                    CardArtsId = "14230200",
                }
            },
            {
                "54005",//矮人佣兵
                new GwentCard()
                {
                    CardId ="54005",
                    EffectType=typeof(DwarvenMercenary),//效果
                    Name="矮人佣兵",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Dwarf},//需要添加
                    Flavor = "工作就该既有趣又赚钱——比如拿悬赏换金子。",
                    Info = "将1个单位移至它所在战场的同排。若为友军单位，则使它获得3点增益。",
                    CardArtsId = "14231100",
                }
            },
            {
                "54006",//先知
                new GwentCard()
                {
                    CardId ="54006",
                    EffectType=typeof(Farseer),//效果
                    Name="先知",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Elf},//需要添加
                    Flavor = "有时候，他的话语听似令人费解，但其中总是蕴藏着深邃的道理和惊人的智慧。",
                    Info = "己方回合中，若有除自身外的友军单位或手牌中的单位获得增益，则回合结束时获得2点增益。",
                    CardArtsId = "20013600",
                }
            },
            {
                "54007",//维里赫德旅骑兵
                new GwentCard()
                {
                    CardId ="54007",
                    EffectType=typeof(VriheddDragoon),//效果
                    Name="维里赫德旅骑兵",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "我见过的最可怕的场景？卡特利欧纳瘟疫、范格堡被夷为平地，还有维里赫德旅骑兵的冲锋。",
                    Info = "回合结束时，使手牌中1张随机非间谍单位牌获得1点增益。",
                    CardArtsId = "14220500",
                }
            },
            {
                "54008",//多尔·布雷坦纳弓箭手
                new GwentCard()
                {
                    CardId ="54008",
                    EffectType=typeof(DolBlathannaArcher),//效果
                    Name="多尔·布雷坦纳弓箭手",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "再走一步试试，人类。你眉宇间插根箭肯定好看得多。",
                    Info = "分别造成3、1点伤害。",
                    CardArtsId = "14231000",
                }
            },
            {
                "54009",//多尔·布雷坦纳射手
                new GwentCard()
                {
                    CardId ="54009",
                    EffectType=typeof(DolBlathannaBowman),//效果
                    Name="多尔·布雷坦纳射手",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "也许你能躲过他们，但要被发现了，就别浪费时间逃跑了。",
                    Info = "对1个敌军单位造成2点伤害。 每当有敌军单位改变所在排别，便对其造成2点伤害。 自身移动时对1个敌军随机单位造成2点伤害。",
                    CardArtsId = "14231400",
                }
            },
            {
                "54010",//私枭走私者
                new GwentCard()
                {
                    CardId ="54010",
                    EffectType=typeof(HawkerSmuggler),//效果
                    Name="私枭走私者",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Elf,Categorie.Support},//需要添加
                    Flavor = "谁付的钱多我就给谁卖命，不然就挑个最容易抢的去抢。",
                    Info = "每有1个敌军单位被打出，便获得1点增益。",
                    CardArtsId = "14231500",
                }
            },
            {
                "54011",//私枭后援者
                new GwentCard()
                {
                    CardId ="54011",
                    EffectType=typeof(HawkerSupport),//效果
                    Name="私枭后援者",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Elf,Categorie.Support},//需要添加
                    Flavor = "矮人和精灵在我眼里都一样，给钱就行。",
                    Info = "使手牌中1张单位牌获得3点增益。",
                    CardArtsId = "14231200",
                }
            },
            {
                "54012",//玛哈坎劫掠者
                new GwentCard()
                {
                    CardId ="54012",
                    EffectType=typeof(MahakamMarauder),//效果
                    Name="玛哈坎劫掠者",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Dwarf},//需要添加
                    Flavor = "在玛哈坎崎岖的悬崖峭壁上狩猎可不是件简单事……但矮人们也从不轻易向危险低头。",
                    Info = "战力改变时（被重置除外），获得2点增益。",
                    CardArtsId = "20004200",
                }
            },
            {
                "54013",//维里赫德旅先锋
                new GwentCard()
                {
                    CardId ="54013",
                    EffectType=typeof(VriheddVanguard),//效果
                    Name="维里赫德旅先锋",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "管他泰莫利亚人还是瑞达尼亚人，杀无赦。",
                    Info = "使所有“精灵”友军获得1点增益。 每次被交换时，再次触发此能力。",
                    CardArtsId = "16230900",
                }
            },
            {
                "54014",//多尔·布雷坦纳爆破手
                new GwentCard()
                {
                    CardId ="54014",
                    EffectType=typeof(DolBlathannaBomber),//效果
                    Name="多尔·布雷坦纳爆破手",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "他们的追踪本领犹如猎犬，双腿健似矫鹿，残忍更胜恶魔。",
                    Info = "在对方单排生成1个“焚烧陷阱”。",
                    CardArtsId = "14230400",
                }
            },
            {
                "54015",//矮人好斗分子
                new GwentCard()
                {
                    CardId ="54015",
                    EffectType=typeof(DwarvenSkirmisher),//效果
                    Name="矮人好斗分子",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Dwarf},//需要添加
                    Flavor = "我跟十字镐打了一辈子交道，动动斧头算什么问题？",
                    Info = "对1个敌军单位造成3点伤害。若没有摧毁目标，则获得3点增益。",
                    CardArtsId = "14230500",
                }
            },
            {
                "54016",//玛哈坎捍卫者
                new GwentCard()
                {
                    CardId ="54016",
                    EffectType=typeof(MahakamDefender),//效果
                    Name="玛哈坎捍卫者",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Dwarf},//需要添加
                    Flavor = "听好了，我们是天生的战士——拳拳到肉，绝不留情！",
                    Info = "坚韧。",
                    CardArtsId = "16230600",
                }
            },
            {
                "54017",//半精灵猎人
                new GwentCard()
                {
                    CardId ="54017",
                    EffectType=typeof(HalfElfHunter),//效果
                    Name="半精灵猎人",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "被人类痛恨，被精灵唾骂，而且学校操场上谁都不肯带他们玩。难怪半精灵一肚子委屈。",
                    Info = "生成1张佚亡原始同名牌。",
                    CardArtsId = "20163600",
                }
            },
            {
                "54018",//私枭治疗者
                new GwentCard()
                {
                    CardId ="54018",
                    EffectType=typeof(HawkerHealer),//效果
                    Name="私枭治疗者",
                    Strength=5,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Elf,Categorie.Support},//需要添加
                    Flavor = "帮你包扎，没问题——只要你有钱。",
                    Info = "使2个友军单位获得3点增益。",
                    CardArtsId = "14230100",
                }
            },
            {
                "54019",//烟火技师
                new GwentCard()
                {
                    CardId ="54019",
                    EffectType=typeof(Pyrotechnician),//效果
                    Name="烟火技师",
                    Strength=5,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Dwarf},//需要添加
                    Flavor = "在玛哈坎，这份行当的风险非同一般，因此回报也异常优厚。其中最负盛名的行业翘楚，当数那位名叫麦柯尔·贝的矮人。",
                    Info = "对对方每排的1个随即单位造成3点伤害。",
                    CardArtsId = "20013500",
                }
            },
            {
                "54020",//维里赫德旅军官
                new GwentCard()
                {
                    CardId ="54020",
                    EffectType=typeof(VriheddOfficer),//效果
                    Name="维里赫德旅军官",
                    Strength=5,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Elf,Categorie.Officer},//需要添加
                    Flavor = "“仇恨之火比地狱烈焰更猛烈，比任何伤口都更刻骨铭心。”",
                    Info = "交换1张牌，获得等同于它基础战力的增益。",
                    CardArtsId = "14230300",
                }
            },
            {
                "54021",//精灵剑术大师
                new GwentCard()
                {
                    CardId ="54021",
                    EffectType=typeof(ElvenSwordmaster),//效果
                    Name="精灵剑术大师",
                    Strength=5,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "战斗如同舞蹈，千万不能让你的对手领舞。",
                    Info = "对1个敌军单位造成等同自身战力的伤害。",
                    CardArtsId = "20156400",
                }
            },
            {
                "54022",//玛哈坎守卫
                new GwentCard()
                {
                    CardId ="54022",
                    EffectType=typeof(MahakamGuard),//效果
                    Name="玛哈坎守卫",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Dwarf},//需要添加
                    Flavor = "破坏玛哈坎的和平只有一种下场：一记重锤。",
                    Info = "使1个友军单位获得7点增益。",
                    CardArtsId = "14231700",
                }
            },
            {
                "54023",//黑豹
                new GwentCard()
                {
                    CardId ="54023",
                    EffectType=typeof(Panther),//效果
                    Name="黑豹",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast},//需要添加
                    Flavor = "曾经有位牛堡学者在观察一只黑豹后，宣称它不过是颜色不同的花豹而已。黑豹貌似对这一说法毫不关心。他还没等学者完成研究，就把他狼吞虎咽地吃下了肚。",
                    Info = "若对方某排单位少于4个，则对其中1个单位造成7点伤害。",
                    CardArtsId = "20013900",
                }
            },
            {
                "54024",//战舞者
                new GwentCard()
                {
                    CardId ="54024",
                    EffectType=typeof(Wardancer),//效果
                    Name="战舞者",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "你说那个女精灵在大家打得不可开交时跳起舞来了？你疯了吗，下士？！",
                    Info = "被交换时自动打出至随机排。",
                    CardArtsId = "14231300",
                }
            },
            {
                "54025",//蓝山精锐
                new GwentCard()
                {
                    CardId ="54025",
                    EffectType=typeof(BlueMountainElite),//效果
                    Name="蓝山精锐",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "听到他们奔袭的动静时，想跑已来不及了……",
                    Info = "召唤所有同名牌。 自身移动时获得2点增益。",
                    CardArtsId = "14231600",
                }
            },
            {
                "54026",//玛哈坎志愿军
                new GwentCard()
                {
                    CardId ="54026",
                    EffectType=typeof(MahakamVolunteers),//效果
                    Name="玛哈坎志愿军",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Dwarf},//需要添加
                    Flavor = "呼啊！呼啊！把你们的屁股准备好！我们要狠狠地踹一脚！踢得你们夹着尾巴到处跑！",
                    Info = "召唤所有同名牌。",
                    CardArtsId = "20155900",
                }
            },
            {
                "54027",//多尔·布雷坦纳哨兵
                new GwentCard()
                {
                    CardId ="54027",
                    EffectType=typeof(DolBlathannaSentry),//效果
                    Name="多尔·布雷坦纳哨兵",
                    Strength=2,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "只要我们一息尚存，就绝不容许人类践踏多尔·布雷坦纳的绿荫。",
                    Info = "位于己方半场、牌组或手牌：己方打出特殊牌时获得1点增益。",
                    CardArtsId = "20003900",
                }
            },
            {
                "54028",//贤者
                new GwentCard()
                {
                    CardId ="54028",
                    EffectType=typeof(Sage),//效果
                    Name="贤者",
                    Strength=2,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage,Categorie.Elf},//需要添加
                    Flavor = "亲爱的，知识，乃是一种特权。而特权，只能被实力相当的人所分享。",
                    Info = "复活1张铜色“炼金”或“法术”牌，随后将其放逐。",
                    CardArtsId = "20013800",
                }
            },
            {
                "54029",//矮人煽动分子
                new GwentCard()
                {
                    CardId ="54029",
                    EffectType=typeof(DwarvenAgitator),//效果
                    Name="矮人煽动分子",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support,Categorie.Dwarf},//需要添加
                    Flavor = "“记住我说的话，如果你们不行动起来，人类就会抢走我们的姑娘！”",
                    Info = "随机生成1张牌组中非同名铜色“矮人”牌的原始同名牌。",
                    CardArtsId = "20029300",
                }
            },
            {
                "54030",//精灵佣兵
                new GwentCard()
                {
                    CardId ="54030",
                    EffectType=typeof(ElvenMercenary),//效果
                    Name="精灵佣兵",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.Elf},//需要添加
                    Flavor = "我瞧不起松鼠党，但不讨厌他们的钱。",
                    Info = "随机检视牌组中2张铜色“特殊”牌，打出1张。",
                    CardArtsId = "14230800",
                }
            },
            {
                "54031",//精灵利剑
                new GwentCard()
                {
                    CardId ="54031",
                    EffectType=typeof(ElvenBlade),//效果
                    Name="精灵利剑",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "精灵的利剑轻盈，却能造成重伤。",
                    Info = "对1个非“精灵”单位造成10点伤害。",
                    CardArtsId = "20151100",
                }
            },
            {
                "54032",//碎骨陷阱
                new GwentCard()
                {
                    CardId ="54032",
                    EffectType=typeof(CrushingTrap),//效果
                    Name="碎骨陷阱",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "要是被它击倒，你就别想在爬起来了。",
                    Info = "使对方单排左右两侧末端的单位各受到6点伤害。",
                    CardArtsId = "20143900",
                }
            },
            {
                "55001",//焚烧陷阱
                new GwentCard()
                {
                    CardId ="55001",
                    EffectType=typeof(IncineratingTrap),//效果
                    Name="焚烧陷阱",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.ScoiaTael,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Machine,Categorie.Token},//需要添加
                    Flavor = "小心……！再走一步，你就要被化成青烟了。",
                    Info = "对同排除自身外所有单位造成2点伤害，并在回合结束时放逐自身。",
                    CardArtsId = "14330100",
                }
            },
            {
                "62001",//奥拉夫
                new GwentCard()
                {
                    CardId ="62001",
                    EffectType=typeof(Olaf),//效果
                    Name="奥拉夫",
                    Strength=20,
                    Group=Group.Gold,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Cursed},//需要添加
                    Flavor = "备受敬仰的小史凯利格岛竞技场十冠王。",
                    Info = "对自身造成10点伤害。本次对局己方每打出过1只“野兽”，伤害便减少2点。",
                    CardArtsId = "20010300",
                }
            },
            {
                "62002",//哈尔玛·奎特
                new GwentCard()
                {
                    CardId ="62002",
                    EffectType=typeof(Hjalmar),//效果
                    Name="哈尔玛·奎特",
                    Strength=16,
                    Group=Group.Gold,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanAnCraite,Categorie.Officer},//需要添加
                    Flavor = "別为死者哭泣，敬他们一杯吧！",
                    Info = "在对方同排生成“乌德维克之主”。",
                    CardArtsId = "15210100",
                }
            },
            {
                "62003",//维伯约恩
                new GwentCard()
                {
                    CardId ="62003",
                    EffectType=typeof(Vabjorn),//效果
                    Name="维伯约恩",
                    Strength=11,
                    Group=Group.Gold,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Cultist},//需要添加
                    Flavor = "为了斯瓦勃洛！",
                    Info = "对1个单位造成2点伤害。若目标已受伤，则将其摧毁。",
                    CardArtsId = "20002800",
                }
            },
            {
                "62004",//莫斯萨克
                new GwentCard()
                {
                    CardId ="62004",
                    EffectType=typeof(Ermion),//效果
                    Name="莫斯萨克",
                    Strength=10,
                    Group=Group.Gold,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support,Categorie.ClanAnCraite},//需要添加
                    Flavor = "无知者才会轻视神话。",
                    Info = "抽2张牌，随后丢弃2张牌。",
                    CardArtsId = "15210300",
                }
            },
            {
                "62005",//海上野猪
                new GwentCard()
                {
                    CardId ="62005",
                    EffectType=typeof(WildBoarOfTheSea),//效果
                    Name="海上野猪",
                    Strength=8,
                    Group=Group.Gold,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Machine,Categorie.ClanAnCraite},//需要添加
                    Flavor = "只消对尼弗迦德人提起这个名字，他们就会吓得尿裤子……",
                    Info = "回合结束时，使左侧单位获得1点强化，右侧单位收到1点伤害。5点护甲。",
                    CardArtsId = "15210900",
                }
            },
            {
                "62006",//碧尔娜·布兰
                new GwentCard()
                {
                    CardId ="62006",
                    EffectType=typeof(BirnaBran),//效果
                    Name="碧尔娜·布兰",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanTuirseach,Categorie.Officer},//需要添加
                    Flavor = "史凯利格需要一位强大的国王，无论付出何等代价。",
                    Info = "在对方单排降下“史凯利格风暴”。",
                    CardArtsId = "15210500",
                }
            },
            {
                "62007",//凯瑞丝·奎特
                new GwentCard()
                {
                    CardId ="62007",
                    EffectType=typeof(Cerys),//效果
                    Name="凯瑞丝·奎特",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanAnCraite,Categorie.Officer},//需要添加
                    Flavor = "大家叫我小雀鹰，知道为什么吗？因为我专治你这种鼠辈。",
                    Info = "位于墓场中时，在己方复活4个单位后，复活此单位。",
                    CardArtsId = "20017700",
                }
            },
            {
                "62008",//“疯子”卢戈
                new GwentCard()
                {
                    CardId ="62008",
                    EffectType=typeof(MadmanLugos),//效果
                    Name="“疯子”卢戈",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanDrummond,Categorie.Officer},//需要添加
                    Flavor = "哇哇哇哇哇哇啊！！！！",
                    Info = "从牌组丢弃1张铜色单位牌，对1个敌军单位造成等同于被丢弃单位基础战力的伤害。",
                    CardArtsId = "15210600",
                }
            },
            {
                "62009",//乌弗海登
                new GwentCard()
                {
                    CardId ="62009",
                    EffectType=typeof(Ulfhedinn),//效果
                    Name="乌弗海登",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Cursed},//需要添加
                    Flavor = "狼人？哦，不，不……比那要糟糕得多。",
                    Info = "对所有敌军单位造成1点伤害，已受伤单位则承受2点伤害。",
                    CardArtsId = "20010400",
                }
            },
            {
                "62010",//凯瑞丝：无所畏惧
                new GwentCard()
                {
                    CardId ="62010",
                    EffectType=typeof(CerysFearless),//效果
                    Name="凯瑞丝：无所畏惧",
                    Strength=6,
                    Group=Group.Gold,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanAnCraite,Categorie.Officer},//需要添加
                    Flavor = "我必须要团结各大家族。我希望能够避免开战。但假如尼弗迦德执意来犯，那我们就一定要同仇敌忾。",
                    Info = "复活己方下张丢弃的单位牌。",
                    CardArtsId = "20177800",
                }
            },
            {
                "62011",//珊瑚
                new GwentCard()
                {
                    CardId ="62011",
                    EffectType=typeof(Coral),//效果
                    Name="珊瑚",
                    Strength=5,
                    Group=Group.Gold,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Mage},//需要添加
                    Flavor = "她的真名是艾丝翠特·丽塔尼德·艾斯杰芬比约斯道提尔，这名字不管怎么念都拗口极了。",
                    Info = "将1个铜色/银色单位变为“翡翠人偶”。",
                    CardArtsId = "15210700",
                }
            },
            {
                "62012",//希姆
                new GwentCard()
                {
                    CardId ="62012",
                    EffectType=typeof(Hym),//效果
                    Name="希姆",
                    Strength=3,
                    Group=Group.Gold,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed},//需要添加
                    Flavor = "诸神对我说话……我听见他们在暗影中的私语……",
                    Info = "择一：从牌组打出1张铜色/银色“诅咒生物”牌；或创造对方初始牌组中1张银色单位牌。",
                    CardArtsId = "20010200",
                }
            },
            {
                "62013",//坎比
                new GwentCard()
                {
                    CardId ="62013",
                    EffectType=typeof(Kambi),//效果
                    Name="坎比",
                    Strength=1,
                    Group=Group.Gold,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{},//需要添加
                    Flavor = "终焉之刻来临时，金公鸡坎比便会叫醒沉睡的汉姆多尔。",
                    Info = "间谍。遗愿：生成“汉姆多尔”。",
                    CardArtsId = "15210400",
                }
            },
            {
                "63001",//茱塔·迪门
                new GwentCard()
                {
                    CardId ="63001",
                    EffectType=typeof(JuttaAnDimun),//效果
                    Name="茱塔·迪门",
                    Strength=13,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.ClanDimun},//需要添加
                    Flavor = "有人称她“铁娘子”。",
                    Info = "对自身造成1点伤害。",
                    CardArtsId = "15220800",
                }
            },
            {
                "63002",//乌达瑞克
                new GwentCard()
                {
                    CardId ="63002",
                    EffectType=typeof(Udalryk),//效果
                    Name="乌达瑞克",
                    Strength=13,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Agent,Categorie.ClanBrokvar},//需要添加
                    Flavor = "诸神已经发话，必须献上祭品。",
                    Info = "间谍、力竭。 检视牌组中2张牌。抽取1张，丢弃另1张。",
                    CardArtsId = "15221400",
                }
            },
            {
                "63003",//邓戈·费特
                new GwentCard()
                {
                    CardId ="63003",
                    EffectType=typeof(DjengeFrett),//效果
                    Name="邓戈·费特",
                    Strength=10,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.ClanDimun},//需要添加
                    Flavor = "如果通缉令上写着“无论死活”，绝大多数赏金猎人会直接快刀斩乱麻。但我不会。如果被我抓到，我会把人吊起来挠痒，让他笑到岔气。",
                    Info = "对2个友军单位造成1点伤害。每影响一个单位，便获得2点强化。",
                    CardArtsId = "15220300",
                }
            },
            {
                "63004",//“阿蓝”卢戈
                new GwentCard()
                {
                    CardId ="63004",
                    EffectType=typeof(BlueboyLugos),//效果
                    Name="“阿蓝”卢戈",
                    Strength=9,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanDrummond,Categorie.Soldier},//需要添加
                    Flavor = "我无聊得快吐了。",
                    Info = "在对方单排生成1只“幽灵鲸”。",
                    CardArtsId = "15220100",
                }
            },
            {
                "63005",//莫克瓦格
                new GwentCard()
                {
                    CardId ="63005",
                    EffectType=typeof(Morkvarg),//效果
                    Name="莫克瓦格",
                    Strength=9,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Cursed},//需要添加
                    Flavor = "史凯利格有史以来最大的恶人。",
                    Info = "进入墓场时，复活自身，但战力削弱一半。",
                    CardArtsId = "15220900",
                }
            },
            {
                "63006",//斯凡瑞吉·图尔赛克
                new GwentCard()
                {
                    CardId ="63006",
                    EffectType=typeof(SvanrigeTuirseach),//效果
                    Name="斯凡瑞吉·图尔赛克",
                    Strength=9,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanTuirseach,Categorie.Officer},//需要添加
                    Flavor = "皇帝最开始也认为自己登上皇位是出于偶然。",
                    Info = "抽1张牌，随后丢弃1张牌。",
                    CardArtsId = "15221300",
                }
            },
            {
                "63007",//多纳·印达
                new GwentCard()
                {
                    CardId ="63007",
                    EffectType=typeof(Donar),//效果
                    Name="多纳·印达",
                    Strength=8,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanHeymaey,Categorie.Officer},//需要添加
                    Flavor = "我已齐集众位族长，有话快说。",
                    Info = "改变1个单位的锁定状态。从对方墓场中1张铜色单位牌移至己方墓场。",
                    CardArtsId = "15220400",
                }
            },
            {
                "63008",//大野猪
                new GwentCard()
                {
                    CardId ="63008",
                    EffectType=typeof(GiantBoar),//效果
                    Name="大野猪",
                    Strength=8,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast},//需要添加
                    Flavor = "如果在林子里捡到一头硕大的野猪，大多数人会尿了裤子，手忙脚乱地朝最近的树上爬。史凯利格人不会。他们反回两眼发直，大流口水。",
                    Info = "随机摧毁1个友军单位，然后获得10点增益。",
                    CardArtsId = "20162300",
                }
            },
            {
                "63009",//至尊冠军
                new GwentCard()
                {
                    CardId ="63009",
                    EffectType=typeof(ChampionOfHov),//效果
                    Name="至尊冠军",
                    Strength=7,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Ogroid},//需要添加
                    Flavor = "他只对打败他的人报上名讳，因为他是个巨魔游侠，懂吗？",
                    Info = "与1个敌军单位对决。",
                    CardArtsId = "15220200",
                }
            },
            {
                "63010",//德莱格·波·德乌
                new GwentCard()
                {
                    CardId ="63010",
                    EffectType=typeof(DraigBonDhu),//效果
                    Name="德莱格·波·德乌",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support},//需要添加
                    Flavor = "竖起耳朵，来听上一听奎特家族的英雄事迹吧！",
                    Info = "使墓场中2个单位获得3点强化。",
                    CardArtsId = "15220500",
                }
            },
            {
                "63011",//“黑手”霍格
                new GwentCard()
                {
                    CardId ="63011",
                    EffectType=typeof(HolgerBlackhand),//效果
                    Name="“黑手”霍格",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanDimun,Categorie.Officer},//需要添加
                    Flavor = "敬尼弗迦德皇帝，祝他不得善终！",
                    Info = "造成6点伤害。若摧毁目标，则使己方墓场中最强的单位获得3点强化。",
                    CardArtsId = "15220700",
                }
            },
            {
                "63012",//哈罗德·霍兹诺特
                new GwentCard()
                {
                    CardId ="63012",
                    EffectType=typeof(HaraldHoundsnout),//效果
                    Name="哈罗德·霍兹诺特",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.ClanTordarroch},//需要添加
                    Flavor = "曾经是托达洛克家族的首领，如今只是一个喋喋不休的疯子。",
                    Info = "生成“威尔弗雷德”，“威尔海姆”，“威尔玛”。",
                    CardArtsId = "20004300",
                }
            },
            {
                "63013",//尤娜
                new GwentCard()
                {
                    CardId ="63013",
                    EffectType=typeof(Yoana),//效果
                    Name="尤娜",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanTordarroch,Categorie.Support},//需要添加
                    Flavor = "没人会把我当成一名老练的盔甲师傅。只是一个人类，而且还是个女人。可是矮人铁匠就不同了……",
                    Info = "治愈1个友军单位，随后使其获得等同于治疗量的增益。",
                    CardArtsId = "20164400",
                }
            },
            {
                "63014",//迪兰
                new GwentCard()
                {
                    CardId ="63014",
                    EffectType=typeof(Derran),//效果
                    Name="迪兰",
                    Strength=6,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.ClanTuirseach},//需要添加
                    Flavor = "能引发这样的疯狂，相比是极其可怖的……",
                    Info = "每当1个敌军单位受到伤害，便获得1点增益。",
                    CardArtsId = "6010400",
                }
            },
            {
                "63015",//史凯裘
                new GwentCard()
                {
                    CardId ="63015",
                    EffectType=typeof(Skjall),//效果
                    Name="史凯裘",
                    Strength=5,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.ClanHeymaey},//需要添加
                    Flavor = "把他剔出族谱！不许任何人给他食物和庇护！",
                    Info = "从牌组随机打出1张铜色/银色“诅咒生物”单位牌。",
                    CardArtsId = "20021200",
                }
            },
            {
                "63016",//格雷密斯特
                new GwentCard()
                {
                    CardId ="63016",
                    EffectType=typeof(Gremist),//效果
                    Name="格雷密斯特",
                    Strength=4,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support},//需要添加
                    Flavor = "精通炼金术的大德鲁伊，也是群岛脾气最差的老混蛋。",
                    Info = "部署：生成“倾盆大雨”、“晴空”或“惊悚咆哮”。",
                    CardArtsId = "15220600",
                }
            },
            {
                "63017",//茜格德莉法
                new GwentCard()
                {
                    CardId ="63017",
                    EffectType=typeof(Sigrdrifa),//效果
                    Name="茜格德莉法",
                    Strength=3,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support,Categorie.Doomed},//需要添加
                    Flavor = "跪在我身边，向圣母低头。",
                    Info = "复活1个铜色/银色“家族”单位。",
                    CardArtsId = "15221100",
                }
            },
            {
                "63018",//史璀伯格符文石
                new GwentCard()
                {
                    CardId ="63018",
                    EffectType=typeof(StribogRunestone),//效果
                    Name="史璀伯格符文石",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Alchemy,Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "欧菲尔的符文大师可以把它们组合成威力无比的符文。",
                    Info = "创造1张铜色/银色“史凯利格”牌。",
                    CardArtsId = "20158100",
                }
            },
            {
                "63019",//回复
                new GwentCard()
                {
                    CardId ="63019",
                    EffectType=typeof(Restore),//效果
                    Name="回复",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Spell,Categorie.Special},//需要添加
                    Flavor = "那些该死的女术士又抢咱们的风头！只要几个年轻姑娘挥挥手就能解决的话，谁还选择这么费时费力的办法？",
                    Info = "将墓场1张铜色/银色“史凯利格”单位牌置入手牌，为其添加佚亡标签，再将其基础战力设为8点，随后打出1张牌。",
                    CardArtsId = "15320100",
                }
            },
            {
                "63020",//华美的长剑
                new GwentCard()
                {
                    CardId ="63020",
                    EffectType=typeof(OrnamentalSword),//效果
                    Name="华美的长剑",
                    Strength=0,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "看上去很精美，但顶多也就只能用来抹抹黄油。",
                    Info = "创造1个铜色/银色史凯利格“士兵”单位，并使其获得3点强化。",
                    CardArtsId = "20164200",
                }
            },
            {
                "64001",//奎特家族战士
                new GwentCard()
                {
                    CardId ="64001",
                    EffectType=typeof(AnCraiteWarrior),//效果
                    Name="奎特家族战士",
                    Strength=12,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.ClanAnCraite},//需要添加
                    Flavor = "我们的吟游诗人会世代传颂我的功绩，而你死了就会被世人遗忘！",
                    Info = "对自身造成1点伤害。",
                    CardArtsId = "15230300",
                }
            },
            {
                "64002",//迪门家族海盗
                new GwentCard()
                {
                    CardId ="64002",
                    EffectType=typeof(DimunPirate),//效果
                    Name="迪门家族海盗",
                    Strength=11,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.ClanDimun},//需要添加
                    Flavor = "我能看到他们眼中的恐惧。他们害怕我……害怕迪门家族！",
                    Info = "丢弃牌组中所有同名牌。",
                    CardArtsId = "15230500",
                }
            },
            {
                "64003",//海玫家族佛兰明妮卡
                new GwentCard()
                {
                    CardId ="64003",
                    EffectType=typeof(HeymaeyFlaminica),//效果
                    Name="海玫家族佛兰明妮卡",
                    Strength=10,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanHeymaey,Categorie.Support},//需要添加
                    Flavor = "佛兰明妮卡是女德鲁伊最高领袖的头衔，她备受众人的崇敬，拥有无比的力量。",
                    Info = "移除所在排的灾厄，并将2个友军单位移至该排。",
                    CardArtsId = "20014700",
                }
            },
            {
                "64004",//迪门家族走私贩
                new GwentCard()
                {
                    CardId ="64004",
                    EffectType=typeof(DimunSmuggler),//效果
                    Name="迪门家族走私贩",
                    Strength=10,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.ClanDimun},//需要添加
                    Flavor = "史派克鲁格是一片死水，不过没有关系。我们想要什么，就从你们那儿夺。",
                    Info = "将1个铜色单位从己方墓场返回至牌组。",
                    CardArtsId = "20014600",
                }
            },
            {
                "64005",//狂战士掠夺者
                new GwentCard()
                {
                    CardId ="64005",
                    EffectType=typeof(BerserkerMarauder),//效果
                    Name="狂战士掠夺者",
                    Strength=9,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Soldier,Categorie.Cultist},//需要添加
                    Flavor = "把汤乖乖喝完，不然狂战士就会过来，把你给掳走。",
                    Info = "场上每有1个受伤、或为“诅咒生物”的友军单位，便获得1点增益。",
                    CardArtsId = "15230200",
                }
            },
            {
                "64006",//海玫家族诗人
                new GwentCard()
                {
                    CardId ="64006",
                    EffectType=typeof(HeymaeySkald),//效果
                    Name="海玫家族诗人",
                    Strength=9,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanHeymaey,Categorie.Support},//需要添加
                    Flavor = "海玫家族的事迹将流芳千古。",
                    Info = "使所选“家族”的所有友军单位获得1点增益。",
                    CardArtsId = "15230800",
                }
            },
            {
                "64007",//奎特家族盾牌匠
                new GwentCard()
                {
                    CardId ="64007",
                    EffectType=typeof(AnCraiteBlacksmith),//效果
                    Name="奎特家族盾牌匠",
                    Strength=9,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support,Categorie.ClanAnCraite},//需要添加
                    Flavor = "记住我的话：一面好盾能救你的小命。",
                    Info = "使1个友军单位获得2点强化和2点护甲。",
                    CardArtsId = "15231100",
                }
            },
            {
                "64008",//恶熊
                new GwentCard()
                {
                    CardId ="64008",
                    EffectType=typeof(SavageBear),//效果
                    Name="恶熊",
                    Strength=9,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Cursed},//需要添加
                    Flavor = "“驯服”？哈，小子，史凯利格人也许能训练它们，但那跟驯服完全不同……",
                    Info = "对后续打出至对方半场的单位造成1点伤害。",
                    CardArtsId = "15221000",
                }
            },
            {
                "64009",//奎特家族巨剑士
                new GwentCard()
                {
                    CardId ="64009",
                    EffectType=typeof(AnCraiteGreatsword),//效果
                    Name="奎特家族巨剑士",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.ClanAnCraite},//需要添加
                    Flavor = "啊哈哈，你真让我笑掉大牙，北方佬！怎么？我手上这把大家伙，你都不一定拿得动，还想用它对付我？",
                    Info = "每2回合，若受伤，则在回合开始时治愈自身，并获得2点强化。",
                    CardArtsId = "20004000",
                }
            },
            {
                "64010",//图尔赛克家族弓箭手
                new GwentCard()
                {
                    CardId ="64010",
                    EffectType=typeof(TuirseachArcher),//效果
                    Name="图尔赛克家族弓箭手",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.ClanTuirseach},//需要添加
                    Flavor = "你能射中两百步外的移动靶吗？我能，而且是在暴风雨中。",
                    Info = "对3个单位各造成1点伤害。",
                    CardArtsId = "15231500",
                }
            },
            {
                "64011",//德拉蒙家族好战分子
                new GwentCard()
                {
                    CardId ="64011",
                    EffectType=typeof(DrummondWarmonger),//效果
                    Name="德拉蒙家族好战分子",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanDrummond,Categorie.Soldier},//需要添加
                    Flavor = "为大局着想？！战争就是大局，至善至恶，没什么比它更带劲的了！",
                    Info = "从牌组丢弃1张铜色牌。",
                    CardArtsId = "20003600",
                }
            },
            {
                "64012",//图尔赛克家族好斗分子
                new GwentCard()
                {
                    CardId ="64012",
                    EffectType=typeof(TuirseachSkirmisher),//效果
                    Name="图尔赛克家族好斗分子",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.ClanTuirseach},//需要添加
                    Flavor = "记好了：我们对朋友掏心窝，对敌人挥斧子。",
                    Info = "被复活后获得3点强化。",
                    CardArtsId = "15231300",
                }
            },
            {
                "64013",//暴怒的狂战士
                new GwentCard()
                {
                    CardId ="64013",
                    EffectType=typeof(RagingBerserker),//效果
                    Name="暴怒的狂战士",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Soldier,Categorie.Cultist},//需要添加
                    Flavor = "在诗人的歌谣里，鏖战中变身的狂战士跟野熊没两样。",
                    Info = "受伤或被削弱时变为“狂暴的熊”。",
                    CardArtsId = "15230100",
                }
            },
            {
                "64014",//奎特家族捕鲸鱼叉手
                new GwentCard()
                {
                    CardId ="64014",
                    EffectType=typeof(AnCraiteWhaler),//效果
                    Name="奎特家族捕鲸鱼叉手",
                    Strength=8,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Machine,Categorie.ClanAnCraite},//需要添加
                    Flavor = "无论是海上还是港口，他们盯上的目标永远是最漂亮的那个。",
                    Info = "将1个敌军单位移至其所在半场的同排，并使它受到等同于所在排单位数量的伤害。",
                    CardArtsId = "20030000",
                }
            },
            {
                "64015",//奎特家族盔甲匠
                new GwentCard()
                {
                    CardId ="64015",
                    EffectType=typeof(AnCraiteArmorsmith),//效果
                    Name="奎特家族盔甲匠",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support,Categorie.ClanAnCraite},//需要添加
                    Flavor = "你是讨打。",
                    Info = "治愈2个友军单位，并使其获得3点护甲。",
                    CardArtsId = "15231700",
                }
            },
            {
                "64016",//图尔赛克家族老兵
                new GwentCard()
                {
                    CardId ="64016",
                    EffectType=typeof(TuirseachVeteran),//效果
                    Name="图尔赛克家族老兵",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanTuirseach,Categorie.Support},//需要添加
                    Flavor = "我见人所未见，能人所不能。",
                    Info = "使位于手牌、牌组和己方半场除自身外的所有“图尔赛克家族”单位获得1点强化。",
                    CardArtsId = "20004600",
                }
            },
            {
                "64017",//迪门家族轻型长船
                new GwentCard()
                {
                    CardId ="64017",
                    EffectType=typeof(DimunLightLongship),//效果
                    Name="迪门家族轻型长船",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanDimun,Categorie.Machine},//需要添加
                    Flavor = "你以为能在史凯利格海域逃出他们的手掌心？自求多福吧。",
                    Info = "回合结束时，对右侧的单位造成1点伤害，自身获得2点增益。",
                    CardArtsId = "15230900",
                }
            },
            {
                "64018",//奎特家族作战长船
                new GwentCard()
                {
                    CardId ="64018",
                    EffectType=typeof(AnCraiteLongship),//效果
                    Name="奎特家族作战长船",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Machine,Categorie.ClanAnCraite},//需要添加
                    Flavor = "据说只要有长船出海劫掠，汉姆多尔就会心潮澎湃。",
                    Info = "对1个敌军随机单位造成2点伤害。己方每丢弃1张牌，便触发此能力一次。",
                    CardArtsId = "15231400",
                }
            },
            {
                "64019",//迪门家族战船
                new GwentCard()
                {
                    CardId ="64019",
                    EffectType=typeof(DimunWarship),//效果
                    Name="迪门家族战船",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanDimun,Categorie.Machine},//需要添加
                    Flavor = "迪门家族的战船轻盈迅捷，最适合追逐缓慢笨重的商船。",
                    Info = "连续4次对同一个单位造成1点伤害。",
                    CardArtsId = "20010500",
                }
            },
            {
                "64020",//奎特家族劫掠者
                new GwentCard()
                {
                    CardId ="64020",
                    EffectType=typeof(AnCraiteMarauder),//效果
                    Name="奎特家族劫掠者",
                    Strength=7,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.ClanAnCraite},//需要添加
                    Flavor = "你疯了不成？你想去史凯利格？哪些野蛮人会让你吃大苦头的！",
                    Info = "造成4点伤害。若被复活，则造成6点伤害。",
                    CardArtsId = "20157800",
                }
            },
            {
                "64021",//图尔赛克家族猎人
                new GwentCard()
                {
                    CardId ="64021",
                    EffectType=typeof(TuirseachHunter),//效果
                    Name="图尔赛克家族猎人",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.ClanTuirseach},//需要添加
                    Flavor = "别怀疑我们的狩猎本领，只恨史派克鲁格的猎物太少……",
                    Info = "造成5点伤害。",
                    CardArtsId = "15230400",
                }
            },
            {
                "64022",//图尔赛克家族斧兵
                new GwentCard()
                {
                    CardId ="64022",
                    EffectType=typeof(TuirseachAxeman),//效果
                    Name="图尔赛克家族斧兵",
                    Strength=6,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.ClanTuirseach},//需要添加
                    Flavor = "小姑娘才用剑，弄把斧头吧。",
                    Info = "对方同排每有1个敌军单位受到伤害，便获得1点增益。2点护甲。",
                    CardArtsId = "15231200",
                }
            },
            {
                "64023",//奎特家族战吼者
                new GwentCard()
                {
                    CardId ="64023",
                    EffectType=typeof(AnCraiteWarcrier),//效果
                    Name="奎特家族战吼者",
                    Strength=5,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Support,Categorie.ClanAnCraite},//需要添加
                    Flavor = "有人为泰莫利亚抛头颅，有人为尼弗迦德洒热血。我只为骑士的誓言而战。",
                    Info = "使1个友军单位获得自身一半战力的增益。",
                    CardArtsId = "11331300",
                }
            },
            {
                "64024",//奎特家族突袭者
                new GwentCard()
                {
                    CardId ="64024",
                    EffectType=typeof(AnCraiteRaider),//效果
                    Name="奎特家族突袭者",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.ClanAnCraite},//需要添加
                    Flavor = "我们可是奎特家族！別人用金子购买，我们拿血汗交换。",
                    Info = "被丢弃时复活自身。",
                    CardArtsId = "15231600",
                }
            },
            {
                "64025",//德拉蒙家族女王卫队
                new GwentCard()
                {
                    CardId ="64025",
                    EffectType=typeof(DrummondQueensguard),//效果
                    Name="德拉蒙家族女王卫队",
                    Strength=4,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanDrummond,Categorie.Soldier},//需要添加
                    Flavor = "史凯利格的女王向来由最勇猛、最凶悍的持盾女卫保护。",
                    Info = "复活所有同名牌。",
                    CardArtsId = "15230710",
                }
            },
            {
                "64026",//德拉蒙家族持盾女卫
                new GwentCard()
                {
                    CardId ="64026",
                    EffectType=typeof(DrummondShieldmaid),//效果
                    Name="德拉蒙家族持盾女卫",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanDrummond,Categorie.Soldier},//需要添加
                    Flavor = "我们的敌人会像打上嶙峋海岸的波浪一样，倒在我们的盾前。",
                    Info = "召唤所有同名牌。",
                    CardArtsId = "15231810",
                }
            },
            {
                "64027",//海玫家族草药医生
                new GwentCard()
                {
                    CardId ="64027",
                    EffectType=typeof(HeymaeyHerbalist),//效果
                    Name="海玫家族草药医生",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanHeymaey,Categorie.Support},//需要添加
                    Flavor = "“在史凯利格，我们可不会把聪明的女人绑在柴火上，而是听取她们的建议。”",
                    Info = "从牌组打出1张随机铜色“有机”或灾厄牌。",
                    CardArtsId = "20008100",
                }
            },
            {
                "64028",//迪门家族海贼
                new GwentCard()
                {
                    CardId ="64028",
                    EffectType=typeof(DimunCorsair),//效果
                    Name="迪门家族海贼",
                    Strength=3,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanDimun,Categorie.Support,Categorie.Doomed},//需要添加
                    Flavor = "大海属于我们。海里的东西，不管是漂着的、游着的、划着的，也都是咱们的！",
                    Info = "复活1个铜色“机械”单位。",
                    CardArtsId = "20014500",
                }
            },
            {
                "64029",//海玫家族女矛手
                new GwentCard()
                {
                    CardId ="64029",
                    EffectType=typeof(HeymaeySpearmaiden),//效果
                    Name="海玫家族女矛手",
                    Strength=2,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanHeymaey,Categorie.Support},//需要添加
                    Flavor = "“史凯利格的女人生性狂野，难以捉摸。所有的部队都要把她们视为严重的威胁，绝不能低估她们的实力。”—将军对帝国舰队入侵部队下的指令",
                    Info = "对1个友军“机械”或“士兵”单位造成1点伤害，随后从牌组打出1张它的同名牌。",
                    CardArtsId = "20014800",
                }
            },
            {
                "64030",//海玫家族保卫者
                new GwentCard()
                {
                    CardId ="64030",
                    EffectType=typeof(HeymaeyProtector),//效果
                    Name="海玫家族保卫者",
                    Strength=2,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.ClanHeymaey},//需要添加
                    Flavor = "他天不怕地不怕，除了弗蕾雅的怒火……还有他老婆。",
                    Info = "从牌组打出1张铜色“道具”牌。",
                    CardArtsId = "20014900",
                }
            },
            {
                "64031",//迪门家族海盗船长
                new GwentCard()
                {
                    CardId ="64031",
                    EffectType=typeof(DimunPirateCaptain),//效果
                    Name="迪门家族海盗船长",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanDimun,Categorie.Officer},//需要添加
                    Flavor = "加把劲儿把旗升起来！",
                    Info = "从牌组打出1个非同名铜色“迪门家族”单位。",
                    CardArtsId = "15230600",
                }
            },
            {
                "64032",//弗蕾雅女祭司
                new GwentCard()
                {
                    CardId ="64032",
                    EffectType=typeof(PriestessOfFreya),//效果
                    Name="弗蕾雅女祭司",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.ClanHeymaey,Categorie.Support,Categorie.Doomed},//需要添加
                    Flavor = "圣母弗蕾雅是爱、美与丰饶的女神。",
                    Info = "复活1个铜色“士兵”单位。",
                    CardArtsId = "15231000",
                }
            },
            {
                "64033",//图尔赛克家族驯兽师
                new GwentCard()
                {
                    CardId ="64033",
                    EffectType=typeof(TuirseachBearmaster),//效果
                    Name="图尔赛克家族驯兽师",
                    Strength=1,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Soldier,Categorie.ClanTuirseach},//需要添加
                    Flavor = "别碰他。别盯着他的眼睛瞧。事实上……压根就别靠近他。",
                    Info = "生成1头“熊”。",
                    CardArtsId = "20014400",
                }
            },
            {
                "64034",//骨制护符
                new GwentCard()
                {
                    CardId ="64034",
                    EffectType=typeof(BoneTalisman),//效果
                    Name="骨制护符",
                    Strength=0,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.AnyPlace,
                    CardType = CardType.Special,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Special,Categorie.Item},//需要添加
                    Flavor = "有时最普通不过的物件却拥有最为强大的威力。",
                    Info = "择一：复活1个铜色“野兽”或“呓语”单位；或治愈1名友军单位，并使其获得3点强化。",
                    CardArtsId = "20159800",
                }
            },
            {
                "65001",//汉姆多尔
                new GwentCard()
                {
                    CardId ="65001",
                    EffectType=typeof(Hemdall),//效果
                    Name="汉姆多尔",
                    Strength=20,
                    Group=Group.Gold,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Token},//需要添加
                    Flavor = "白霜到来之时，汉姆多尔将吹响战斗的号角。",
                    Info = "摧毁场上所有单位，并移除所有恩泽和灾厄。",
                    CardArtsId = "15240200",
                }
            },
            {
                "65002",//狂暴的熊
                new GwentCard()
                {
                    CardId ="65002",
                    EffectType=typeof(NoneEffect),//效果
                    Name="狂暴的熊",
                    Strength=12,
                    Group=Group.Copper,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Beast,Categorie.Cursed,Categorie.Cultist},//需要添加
                    Flavor = "吼！！！",
                    Info = "没有特殊技能。",
                    CardArtsId = "15240500",
                }
            },
            {
                "65003",//乌德维克之主
                new GwentCard()
                {
                    CardId ="65003",
                    EffectType=typeof(LordOfUndvik),//效果
                    Name="乌德维克之主",
                    Strength=5,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Ogroid,Categorie.Token},//需要添加
                    Flavor = "这个怪物将名门世族托达洛克家族的故乡——乌德维克岛变成了荒凉之地，昔日荣光一去不返……",
                    Info = "遗愿：使“哈尔玛”获得10点增益。",
                    CardArtsId = "15240100",
                }
            },
            {
                "65004",//幽灵鲸
                new GwentCard()
                {
                    CardId ="65004",
                    EffectType=typeof(SpectralWhale),//效果
                    Name="幽灵鲸",
                    Strength=3,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Token},//需要添加
                    Flavor = "“呃，座头鲸应该没那么大。那是头长须鲸。”“嘴那么短的长须鲸？你被药草冲昏了头吗！”",
                    Info = "回合结束时移至随机排，对同排所有其他单位造成1点伤害。 间谍。",
                    CardArtsId = "15240300",
                }
            },
            {
                "65005",//威尔弗雷德
                new GwentCard()
                {
                    CardId ="65005",
                    EffectType=typeof(Wilfred),//效果
                    Name="威尔弗雷德",
                    Strength=1,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Token},//需要添加
                    Flavor = "高个子的是威尔玛。他右边的是威尔弗雷德。结巴的那个是威尔海姆。",
                    Info = "遗愿：使1个友军随机单位获得3点强化。",
                    CardArtsId = "20052500",
                }
            },
            {
                "65006",//威尔海姆
                new GwentCard()
                {
                    CardId ="65006",
                    EffectType=typeof(Wilhelm),//效果
                    Name="威尔海姆",
                    Strength=1,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Token},//需要添加
                    Flavor = "高个子的是威尔玛。他右边的是威尔弗雷德。结巴的那个是威尔海姆。",
                    Info = "遗愿：对对方同排所有单位造成1点伤害。",
                    CardArtsId = "20052500",
                }
            },
            {
                "65007",//威尔玛
                new GwentCard()
                {
                    CardId ="65007",
                    EffectType=typeof(Wilmar),//效果
                    Name="威尔玛",
                    Strength=1,
                    Group=Group.Silver,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.EnemyRow,
                    CardType = CardType.Unit,
                    IsDoomed = true,
                    IsCountdown = false,
                    IsDerive = true,
                    Categories = new Categorie[]{Categorie.Cursed,Categorie.Token},//需要添加
                    Flavor = "高个子的是威尔玛。他右边的是威尔弗雷德。结巴的那个是威尔海姆。",
                    Info = "遗愿：若为对方回合，则在对面此排生成1头“熊”。 间谍。",
                    CardArtsId = "20052500",
                }
            },
            {
                "61001",//“瘸子”哈罗德
                new GwentCard()
                {
                    CardId ="61001",
                    EffectType=typeof(HaraldTheCripple),//效果
                    Name="“瘸子”哈罗德",
                    Strength=6,
                    Group=Group.Leader,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.ClanAnCraite},//需要添加
                    Flavor = "没人知道他的绰号从何而来，更没人敢问。",
                    Info = "对对方同排的1个随机敌军单位造成1点伤害，再重复9次。",
                    CardArtsId = "15110300",
                }
            },
            {
                "61002",//克拉茨·奎特
                new GwentCard()
                {
                    CardId ="61002",
                    EffectType=typeof(CrachAnCraite),//效果
                    Name="克拉茨·奎特",
                    Strength=5,
                    Group=Group.Leader,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.ClanAnCraite},//需要添加
                    Flavor = "尼弗迦德人叫我“蒂斯·伊斯·穆瑞”，也就是海上野猪。他们还用我的名号来吓唬小孩！",
                    Info = "使牌组中最强的非间谍铜色/银色单位牌获得2点强化，随后打出。",
                    CardArtsId = "15110200",
                }
            },
            {
                "61003",//埃斯特·图尔赛克
                new GwentCard()
                {
                    CardId ="61003",
                    EffectType=typeof(EistTuirseach),//效果
                    Name="埃斯特·图尔赛克",
                    Strength=5,
                    Group=Group.Leader,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.ClanTuirseach},//需要添加
                    Flavor = "埃斯特来到辛特拉，本想帮助克拉茨·奎特参加帕薇塔公主的选亲宴，结果自己却赢走了王后的芳心。",
                    Info = "生成1个铜色“图尔赛克家族”的“士兵”单位。",
                    CardArtsId = "20006000",
                }
            },
            {
                "61004",//布兰王
                new GwentCard()
                {
                    CardId ="61004",
                    EffectType=typeof(BranTuirseach),//效果
                    Name="布兰王",
                    Strength=2,
                    Group=Group.Leader,
                    Faction = Faction.Skellige,
                    CardUseInfo = CardUseInfo.MyRow,
                    CardType = CardType.Unit,
                    IsDoomed = false,
                    IsCountdown = false,
                    IsDerive = false,
                    Categories = new Categorie[]{Categorie.Leader,Categorie.ClanTuirseach},//需要添加
                    Flavor = "没人能取代布兰王，但后世定会努力尝试。",
                    Info = "从牌组丢弃最多3张牌，其中的单位牌获得1点强化。",
                    CardArtsId = "15110100",
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
                Strength = x.Strength,
                //-------
                CardId = x.CardId,
                IsCountdown = x.IsCountdown,
                Countdown = x.Countdown,
                IsDoomed = x.IsDoomed,
                CardArtsId = x.CardArtsId,
                CardType = x.CardType,
                CardUseInfo = x.CardUseInfo,
            });
        }
    }
}