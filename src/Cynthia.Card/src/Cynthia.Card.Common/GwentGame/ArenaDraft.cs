using System;
using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card
{
    //Arena draft rules, rebuilt after the 0.9.24 beta / live Arena:
    //pick 1 is always a gold set, picks 2-26 are a gold, silver or bronze set (1/3 each),
    //every set holds 4 different cards of one rarity drawn fresh from the full pool (so duplicates across sets are possible),
    //and the last pick is 1 of 3 leaders.
    public static class ArenaDraft
    {
        public const int CardPicks = 26;
        public const int TotalPicks = CardPicks + 1;
        public const int CardsPerSet = 4;
        public const int LeadersPerSet = 3;
        public const int WinsToFinish = 9;
        public const int LossesToFinish = 3;

        //relative weights for picks 2-26
        public static int GoldWeight { get; set; } = 1;
        public static int SilverWeight { get; set; } = 1;
        public static int BronzeWeight { get; set; } = 1;

        public static bool IsLeaderPick(int pickNumber) => pickNumber == TotalPicks;

        public static IList<string> Pool(Group group) => GwentMap.CardMap.Values
            .Where(x => !x.IsDerive && x.Group == group)
            .Select(x => x.CardId)
            .ToList();

        public static Group RollGroup(int pickNumber, Random random)
        {
            if (IsLeaderPick(pickNumber))
                return Group.Leader;
            if (pickNumber == 1)
                return Group.Gold;
            var roll = random.Next(GoldWeight + SilverWeight + BronzeWeight);
            if (roll < GoldWeight)
                return Group.Gold;
            if (roll < GoldWeight + SilverWeight)
                return Group.Silver;
            return Group.Copper;
        }

        public static List<string> GenerateChoices(int pickNumber, Random random)
        {
            var group = RollGroup(pickNumber, random);
            var count = group == Group.Leader ? LeadersPerSet : CardsPerSet;
            return Pool(group).OrderBy(_ => random.Next()).Take(count).ToList();
        }

        public static bool IsFinished(ArenaRunModel run) => run.Wins >= WinsToFinish || run.Losses >= LossesToFinish;

        public static bool IsValidArenaDeck(DeckModel deck) =>
            deck.Deck.Count == CardPicks &&
            GwentMap.CardMap.ContainsKey(deck.Leader) && GwentMap.CardMap[deck.Leader].Group == Group.Leader &&
            deck.Deck.All(x => GwentMap.CardMap.ContainsKey(x) && GwentMap.CardMap[x].Group != Group.Leader);
    }
}
