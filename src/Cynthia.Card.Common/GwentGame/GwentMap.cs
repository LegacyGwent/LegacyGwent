using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card
{
    public static class GwentMap
    {
        public static IDictionary<string, GwentCard> CardMap { get; } = new Dictionary<string, GwentCard>
        {
            {"tl",new GwentCard(){Strength=18,Group=Group.Leader,Faction = Faction.Nilfgaard,Name="基础领袖"}},
            {"tg1",new GwentCard(){Strength=17,Group=Group.Gold,Faction = Faction.Neutral,Name="基础金卡一号"}},
            {"tg2",new GwentCard(){Strength=16,Group=Group.Gold,Faction = Faction.Neutral,Name="基础金卡二号"}},
            {"tg3",new GwentCard(){Strength=15,Group=Group.Gold,Faction = Faction.Neutral,Name="基础金卡三号"}},
            {"tg4",new GwentCard(){Strength=14,Group=Group.Gold,Faction = Faction.Neutral,Name="基础金卡四号"}},
            {"ts1",new GwentCard(){Strength=13,Group=Group.Silver,Faction = Faction.Neutral,Name = "基础银卡一号"}},
            {"ts2",new GwentCard(){Strength=12,Group=Group.Silver,Faction = Faction.Neutral,Name = "基础银卡二号"}},
            {"ts3",new GwentCard(){Strength=11,Group=Group.Silver,Faction = Faction.Neutral,Name = "基础银卡三号"}},
            {"ts4",new GwentCard(){Strength=10,Group=Group.Silver,Faction = Faction.Neutral,Name = "基础银卡四号"}},
            {"ts5",new GwentCard(){Strength=9,Group=Group.Silver,Faction = Faction.Neutral,Name = "基础银卡五号"}},
            {"ts6",new GwentCard(){Strength=8,Group=Group.Silver,Faction = Faction.Neutral,Name = "基础银卡六号"}},
            {"tc1",new GwentCard(){Strength=7,Group=Group.Copper,Faction = Faction.Neutral,Name = "基础铜卡一号"}},
            {"tc2",new GwentCard(){Strength=6,Group=Group.Copper,Faction = Faction.Neutral,Name = "基础铜卡二号"}},
            {"tc3",new GwentCard(){Strength=5,Group=Group.Copper,Faction = Faction.Neutral,Name = "基础铜卡三号"}},
            {"tc4",new GwentCard(){Strength=4,Group=Group.Copper,Faction = Faction.Neutral,Name = "基础铜卡四号"}},
            {"tc5",new GwentCard(){Strength=3,Group=Group.Copper,Faction = Faction.Neutral,Name = "基础铜卡五号"}},
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