using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card
{
    public static class GwentMap
    {
        public static IDictionary<string, GwentCard> CardMap { get; } = new Dictionary<string, GwentCard>
        {
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