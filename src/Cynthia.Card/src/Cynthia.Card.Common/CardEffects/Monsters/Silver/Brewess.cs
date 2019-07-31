using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23004")]//煮婆
    public class Brewess : CardEffect
    {//召唤“呢喃婆”和“织婆”。
        public Brewess(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var myDeck = Game.PlayersDeck[PlayerIndex].ToList();
            var whispess = myDeck.Where(x => x.Status.CardId == CardId.Whispess).ToList();
            var weavess = myDeck.Where(x => x.Status.CardId == CardId.Weavess).ToList();
            foreach (var whispes in whispess)
            {
                await whispes.Effect.Summon(Card.GetLocation(), Card);
            }
            foreach (var weaves in weavess)
            {
                await weaves.Effect.Summon(Card.GetLocation() + 1, Card);
            }
            return 0;
        }
    }
}