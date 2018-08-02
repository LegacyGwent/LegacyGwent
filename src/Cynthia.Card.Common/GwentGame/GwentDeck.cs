using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Alsein.Utilities;

namespace Cynthia.Card
{
    public static class GwentDeck
    {
        public static DeckModel CreateBasicDeck()
        {
            var leader = "tl";
            var deck = "tg".Plural(4)
                .Concat("ts".Plural(6))
                .Concat("tc".Plural(15)).ToArray();
            return new DeckModel() { Leader = leader, Deck = deck, Name = "初始卡组" };
        }
        public static bool CreateBasicDeck(DeckModel deck)
        {
            var decks = deck.Deck.Select(x => GwentMap.CardMap[x]);
            return true;
        }
    }
}