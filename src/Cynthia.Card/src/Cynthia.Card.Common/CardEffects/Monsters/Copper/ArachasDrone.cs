using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24035")]//蟹蜘蛛雄蛛
    public class ArachasDrone : CardEffect
    {//召唤所有同名牌。
        public ArachasDrone(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
            foreach (var card in cards)
            {
                await card.Effect.Summon(Card.GetLocation() + 1, Card);
            }
            return 0;
        }
    }
}