using System.Collections.Generic;
using System;
using System.Linq;

namespace Cynthia.Card
{
    // Separate from UserInfo so legacy whole-user replacements cannot overwrite the wallet.
    public sealed class PremiumCollection
    {
        public string Id { get; set; }
        public long MeteoritePowder { get; set; }
        public long Revision { get; set; }
        public bool InitialPowderGranted { get; set; }
        public DailyQuestProgress DailyQuests { get; set; }
        public List<string> OwnedCards { get; set; } = new List<string>();
        public List<string> SelectedCards { get; set; } = new List<string>();
        // Version 0 is the original all-copies unlock. Migrate it once on the server.
        public int InventoryVersion { get; set; }
        public Dictionary<string, int> PremiumCopies { get; set; } = new Dictionary<string, int>();
        public List<PremiumCraftReceipt> CraftReceipts { get; set; } = new List<PremiumCraftReceipt>();
        public List<PowderReward> Rewards { get; set; } = new List<PowderReward>();
        public Dictionary<string, PremiumDeckSelection> DeckSelections { get; set; } = new Dictionary<string, PremiumDeckSelection>();
    }

    public sealed class PremiumDeckSelection
    {
        public Dictionary<string, int> PremiumCards { get; set; } = new Dictionary<string, int>();
        public string LeaderId { get; set; }
        public bool PremiumLeader { get; set; }
    }

    public sealed class PremiumCraftReceipt
    {
        public string RequestId { get; set; }
        public string CardId { get; set; }
    }

    public static class CardInventory
    {
        public static int Limit(string cardId) => cardId != null && GwentMap.CardMap.TryGetValue(cardId, out var card)
            ? (card.Group == Group.Copper ? 3 : 1) : 0;

        public static int PremiumCount(PremiumCollection collection, string cardId)
        {
            if (collection == null || cardId == null) return 0;
            if (collection.InventoryVersion == 0)
                return collection.OwnedCards?.Contains(cardId) == true ? Limit(cardId) : 0;
            return collection.PremiumCopies != null && collection.PremiumCopies.TryGetValue(cardId, out var count)
                ? Math.Max(0, Math.Min(Limit(cardId), count)) : 0;
        }

        public static int DeckPremiumCount(DeckModel deck, string cardId) => deck?.PremiumCards != null &&
            cardId != null && deck.PremiumCards.TryGetValue(cardId, out var count) ? count : 0;

        // A null version map denotes a legacy deck; an empty map explicitly selects standard cards.
        public static void InitializeDeck(DeckModel deck, PremiumCollection collection)
        {
            if (deck == null || collection == null) return;
            if (deck.PremiumCards == null)
                deck.PremiumCards = (deck.Deck ?? new List<string>()).GroupBy(x => x)
                    .Where(x => PremiumCount(collection, x.Key) > 0)
                    .ToDictionary(x => x.Key, x => Math.Min(x.Count(), PremiumCount(collection, x.Key)));
            if (!deck.PremiumLeader.HasValue) deck.PremiumLeader = PremiumCount(collection, deck.Leader) > 0;
        }

        public static bool ValidDeckVersions(DeckModel deck, PremiumCollection collection)
        {
            if (deck?.Deck == null || deck.Deck.Any(x => x == null || !GwentMap.CardMap.ContainsKey(x))) return false;
            if (deck.PremiumLeader == true && PremiumCount(collection, deck.Leader) == 0) return false;
            return deck.PremiumCards == null || deck.PremiumCards.All(x => x.Value >= 0 &&
                GwentMap.CardMap.ContainsKey(x.Key) && x.Value <= PremiumCount(collection, x.Key) &&
                x.Value <= deck.Deck.Count(id => id == x.Key));
        }

        public static void TrimDeckVersions(DeckModel deck)
        {
            if (deck.PremiumCards == null) return;
            foreach (var id in deck.PremiumCards.Keys.ToArray())
            {
                int count = Math.Min(deck.PremiumCards[id], deck.Deck.Count(x => x == id));
                if (count <= 0) deck.PremiumCards.Remove(id); else deck.PremiumCards[id] = count;
            }
        }
    }

    public sealed class PowderReward
    {
        public string RewardId { get; set; }
        public long Amount { get; set; }
        public string Reason { get; set; }
        public string GrantedUtc { get; set; }
    }

    public sealed class PremiumCollectionResult
    {
        public string Status { get; set; }
        public PremiumCollection Collection { get; set; }
        public Dictionary<string, int> Costs { get; set; }
        public bool Success => Status == "ok";
    }
}
