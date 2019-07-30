using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23007")]//织婆
    public class Weavess : CardEffect
    {//召唤“呢喃婆”和“煮婆”。
        public Weavess(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var myDeck = Game.PlayersDeck[PlayerIndex].ToList();
            var whispess = myDeck.Where(x => x.Status.CardId == CardId.Whispess).ToList();
            var brewess = myDeck.Where(x => x.Status.CardId == CardId.Brewess).ToList();
            foreach (var whispes in whispess)
            {
                await whispes.Effect.Summon(Card.GetLocation(), Card);
            }
            foreach (var brewes in brewess)
            {
                await brewes.Effect.Summon(Card.GetLocation(), Card);
            }
            return 0;
        }
    }
}