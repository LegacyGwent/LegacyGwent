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
            "70015",
            "70040",
            "70105",
            "130210", "130220", "130200",
            "130180", "130190", "130010", "130150", "130020", "130160", "130130", "130140", "130170", "130110",
            "130090", "130100", "130120", "130080", "130050", "130060", "130070", "130030", "130040", "640080",
            "240140", "240230", "240250",
            "70090",
            "70135",
            "70136",
            "70171",
            "70189",
            "34034", "34035", "34036", "64035", "64036", "64037",
            "70193"
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
