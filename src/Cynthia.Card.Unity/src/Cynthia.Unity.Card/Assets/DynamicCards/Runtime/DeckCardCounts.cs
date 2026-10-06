using System;
using System.Collections.Generic;
using System.Linq;
using Cynthia.Card;

namespace Assets.Script.DynamicCards
{
    // Actual per-group totals of a deck, counted from every real entry of deck.Deck.
    public struct DeckCountSummary
    {
        public int Total;
        public int Gold;
        public int Silver;
        public int Copper;
    }

    // Pure counting/admission arithmetic shared by the deck editor and the match preview.
    // No UnityEngine dependency: the same rules must hold in both screens and in tests.
    public static class DeckCardCounts
    {
        public const int StandardDeckCapacity = 40;
        public const int BlacklistCapacity = 2;
        public const int StandardGoldCapacity = 4;
        public const int SpecialGoldCapacity = 12;
        public const int SilverCapacity = 6;
        public const int MultiCopyCapacity = 3;
        public const int SingleCopyCapacity = 1;

        // Copper copies of either version carry a counter in the editors. Deliberately free of
        // any premium-package gate: a standard-only package shows its standard copper counts.
        public static bool ShowsCopyCount(CardStatus card) => card != null && card.Group == Group.Copper;

        // Same mode inference the editor has always applied on entry.
        public static bool IsSpecial(DeckModel deck) =>
            deck != null && !deck.IsHalfBasicDeck() && deck.IsHalfSpecialDeck();

        public static bool IsBlacklist(DeckModel deck) => deck != null && deck.Id == "blacklist";

        public static int GoldCapacity(bool special) => special ? SpecialGoldCapacity : StandardGoldCapacity;
        public static int GoldCapacity(DeckModel deck) => GoldCapacity(IsSpecial(deck));
        public static int DeckCapacity(DeckModel deck) => IsBlacklist(deck) ? BlacklistCapacity : StandardDeckCapacity;

        // Per-name copy cap. Mirrors CardInventory.Limit, plus the preserved special-gold rule.
        public static int CardCapacity(DeckModel deck, bool special, Group group, string cardId)
        {
            if (IsBlacklist(deck)) return SingleCopyCapacity;
            int limit = CardInventory.Limit(cardId);
            if (special && group == Group.Gold) return Math.Max(limit, MultiCopyCapacity);
            return limit;
        }

        // Leaders and unknown ids are deliberately not part of the gold/silver/copper total.
        public static DeckCountSummary Summarize(DeckModel deck)
        {
            var summary = new DeckCountSummary();
            if (deck == null || deck.Deck == null) return summary;
            foreach (var id in deck.Deck)
            {
                if (id == null || !GwentMap.CardMap.TryGetValue(id, out var card)) continue;
                switch (card.Group)
                {
                    case Group.Gold: summary.Gold++; break;
                    case Group.Silver: summary.Silver++; break;
                    case Group.Copper: summary.Copper++; break;
                    default: continue;
                }
                summary.Total++;
            }
            return summary;
        }

        public static int Total(DeckModel deck) => Summarize(deck).Total;

        public static int GroupCount(DeckModel deck, Group group)
        {
            if (deck == null || deck.Deck == null) return 0;
            int count = 0;
            foreach (var id in deck.Deck)
                if (id != null && GwentMap.CardMap.TryGetValue(id, out var card) && card.Group == group) count++;
            return count;
        }

        public static int CardCount(DeckModel deck, string cardId)
        {
            if (deck == null || deck.Deck == null || cardId == null) return 0;
            int count = 0;
            foreach (var id in deck.Deck) if (id == cardId) count++;
            return count;
        }

        // Copies of one version actually present in the deck. PremiumCards only marks which of
        // the real copies are premium, so the standard share is the remainder of deck.Deck.
        public static int VersionInDeck(DeckModel deck, string cardId, bool premium)
        {
            int premiumCount = Math.Max(0, Math.Min(CardCount(deck, cardId), CardInventory.DeckPremiumCount(deck, cardId)));
            return premium ? premiumCount : CardCount(deck, cardId) - premiumCount;
        }

        // Remaining copies the player may still add of exactly this version: bounded by the
        // owned count of that version, the shared per-name cap, the deck capacity and the
        // gold/silver quality capacity. Premium is a version, not a separate quality.
        public static int AddableCopies(DeckModel deck, string cardId, Group group, bool premium, int ownedCopies)
            => AddableCopies(deck, cardId, group, premium, ownedCopies, IsSpecial(deck));

        public static int AddableCopies(DeckModel deck, string cardId, Group group, bool premium, int ownedCopies, bool special)
        {
            if (deck == null || cardId == null) return 0;
            int remaining = ownedCopies - VersionInDeck(deck, cardId, premium);
            remaining = Math.Min(remaining, CardCapacity(deck, special, group, cardId) - CardCount(deck, cardId));
            remaining = Math.Min(remaining, DeckCapacity(deck) - Total(deck));
            if (!IsBlacklist(deck))
            {
                if (group == Group.Gold) remaining = Math.Min(remaining, GoldCapacity(special) - GroupCount(deck, Group.Gold));
                else if (group == Group.Silver) remaining = Math.Min(remaining, SilverCapacity - GroupCount(deck, Group.Silver));
            }
            return Math.Max(0, remaining);
        }

        // The blacklist stores card identities only, so every candidate is offered exactly once,
        // as the standard version. A leftover premium-only collection filter therefore can
        // neither blank the list nor create a per-version choice.
        public static IList<CardStatus> BlacklistCandidates(IList<CardStatus> cards)
        {
            if (cards == null) return new List<CardStatus>();
            return cards.Where(x => x != null && x.CardId != null)
                .GroupBy(x => x.CardId)
                .Select(group => new CardStatus(group.Key) { IsPremium = false })
                .ToList();
        }
    }
}
