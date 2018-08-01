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
            var deck = "tb".Plural(15)
                .Concat("ts".Plural(6))
                .Concat("tc".Plural(4)).ToArray();
            return new DeckModel() { Leader = leader, Deck = deck };
        }
    }
}