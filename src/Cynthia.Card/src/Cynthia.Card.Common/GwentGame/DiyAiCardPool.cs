using System;
using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card
{
    /// <summary>
    /// Defines the DIY-AI reset card pool without deleting CardMap entries.
    /// Keeping the original key order is required to decode historical deck/result payloads.
    /// </summary>
    public static class DiyAiCardPool
    {
        public static readonly ISet<string> SystemCardIds = new HashSet<string>(StringComparer.Ordinal)
        {
            "70014", "70018", "80001", "80002", "80003",
            "89004", "89005", "89006", "89007", "89008"
        };

        public static readonly ISet<string> RetiredCardIds = new HashSet<string>(StringComparer.Ordinal)
        {
            "70003", "70004",
            "70012", "70013", "70015", "70016", "70017", "70019", "70020", "70021",
            "70024", "70033", "70038", "70039",
            "70040", "70043", "70044", "70046", "70050", "70054",
            "70076", "70077", "70078", "70105",
            "70103", "70104", "70109", "70111",
            "70112", "70114", "70115", "70116", "70117", "70118", "130210", "130220", "130200",
            "130180", "130190", "130010", "130150", "130020", "130160", "130130", "130140", "130170", "130110",
            "130090", "130100", "130120", "130080", "130050", "130060", "130070", "130030", "130040", "640080",
            "240140", "240230", "240250", "70079", "70080", "70081", "70082", "70086",
            "70089", "70090", "70092", "70093", "70094", "70095", "70096", "70097", "70098",
            "70099", "70100", "70101", "70121", "70122", "70123",
            "70126", "70127", "70130", "70159", "70160", "70134", "70135",
            "70136", "70137", "70138", "70139", "70140", "70141", "70142", "70143", "70144",
            "70150", "70151", "70152", "70153",
            "70163", "70165", "70166", "70167",
            "70171", "70173", "70174", "70175",
            "70178", "70184",
            "70188", "70189",
            "34034", "34035", "34036", "64035", "64036", "64037"
        };

        public static bool IsUserDeckCard(string cardId)
        {
            return cardId != null &&
                   GwentMap.CardMap.TryGetValue(cardId, out var card) &&
                   !RetiredCardIds.Contains(cardId) &&
                   !SystemCardIds.Contains(cardId) &&
                   !card.IsDerive;
        }

        public static void Apply(IDictionary<string, GwentCard> cardMap)
        {
            foreach (var cardId in RetiredCardIds)
            {
                if (!cardMap.TryGetValue(cardId, out var card))
                {
                    throw new InvalidOperationException($"Retired DIY card {cardId} is missing from CardMap.");
                }

                card.IsDerive = true;
                cardMap[cardId] = card;
            }

            foreach (var cardId in cardMap.Keys.ToList())
            {
                var card = cardMap[cardId];
                if (card.LinkedCards == null)
                {
                    continue;
                }

                card.LinkedCards = card.LinkedCards
                    .Where(linkedId => !RetiredCardIds.Contains(linkedId))
                    .ToList();
                cardMap[cardId] = card;
            }
        }
    }
}
