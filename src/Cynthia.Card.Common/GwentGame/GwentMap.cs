using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card
{
    public static class GwentMap
    {
        public static IDictionary<string, GwentCard> CardMap { get; } = new Dictionary<string, GwentCard>
        {
            {"tl",new GwentCard(){Strength=18,Group=Group.Leader,Faction = Faction.Nilfgaard}},
            {"tg",new GwentCard(){Strength=15,Group=Group.Gold,Faction = Faction.Neutral}},
            {"ts",new GwentCard(){Strength=13,Group=Group.Silver,Faction = Faction.Neutral}},
            {"tc",new GwentCard(){Strength=10,Group=Group.Copper,Faction = Faction.Neutral}}
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