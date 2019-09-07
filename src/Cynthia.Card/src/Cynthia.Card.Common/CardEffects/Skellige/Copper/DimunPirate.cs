using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64002")]//迪门家族海盗
    public class DimunPirate : CardEffect
    {//丢弃牌组中所有同名牌。
        public DimunPirate(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var card in cards)
            {
                await card.Effect.Discard(Card);
            }
            return 0;
        }
    }
}