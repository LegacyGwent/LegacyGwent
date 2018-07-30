using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Alsein.Utilities;

namespace Cynthia.Card
{
    public class GwentDeck : IReadOnlyList<GwentCard>
    {
        private GwentCard[] _deck;
        public GwentCard Leader { get; private set; }
        public GwentDeck()
        {
            CreateBasicDeck();
        }
        public GwentDeck(IEnumerable<GwentCard> deck, GwentCard leader)
        {
            _deck = deck.ToArray();
            Leader = leader;
        }
        private void CreateBasicDeck()
        {
            Leader = new GwentCard() { Strength = 18, Flavor = Flavor.Leader };
            _deck = new GwentCard { Strength = 8, Flavor = Flavor.Copper }.Plural(15)
                .Concat(new GwentCard { Strength = 13, Flavor = Flavor.Silver }.Plural(6))
                .Concat(new GwentCard { Strength = 15, Flavor = Flavor.Gold }.Plural(4)).ToArray();
        }

        public int Count => ((IReadOnlyList<GwentCard>)_deck).Count;
        public GwentCard this[int index] => ((IReadOnlyList<GwentCard>)_deck)[index];
        public IEnumerator<GwentCard> GetEnumerator() => ((IReadOnlyList<GwentCard>)_deck).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IReadOnlyList<GwentCard>)_deck).GetEnumerator();
    }
}