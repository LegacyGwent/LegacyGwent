using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23008")]//呢喃婆
    public class Whispess : CardEffect
    {//召唤“煮婆”和“织婆”。
        public Whispess(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var myDeck = Game.PlayersDeck[PlayerIndex].ToList();
            var brewess = myDeck.Where(x => x.Status.CardId == CardId.Brewess).ToList();
            var weavess = myDeck.Where(x => x.Status.CardId == CardId.Weavess).ToList();
            foreach (var weaves in weavess)
            {
                await weaves.Effect.Summon(Card.GetLocation() + 1, Card);
            }
            foreach (var brewes in brewess)
            {
                await brewes.Effect.Summon(Card.GetLocation() + 1, Card);
            }
            return 0;
        }
    }
}