using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card
{
    public static class GwentMap
    {
        public static IDictionary<string, GwentCard> CardMap { get; } = new Dictionary<string, GwentCard>
        {
            {"tl", new GwentCard(){Strength=18,Flavor=Flavor.Leader} },
            {"tg",new GwentCard(){Strength=15,Flavor=Flavor.Gold}},
            {"ts",new GwentCard(){Strength=13,Flavor=Flavor.Silver}},
            {"tc",new GwentCard(){Strength=10,Flavor=Flavor.Copper}}
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
                Positions = x.Positions
            });
        }
        public static IDictionary<Flavor, string> FlavorMap { get; } = new Dictionary<Flavor, string>
        {
            { Flavor.Leader, "领袖" },
            { Flavor.Gold,"金"},
            {Flavor.Silver,"银"},
            {Flavor.Copper,"铜"}
        };
    }
}