using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23022")]//寄生虫
    public class Parasite : CardEffect
    {//对1个敌军单位造成12点伤害；或使1个友军单位获得12点增益。
        public Parasite(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var cards = await Game.GetSelectPlaceCards(Card);

            if (!cards.TrySingle(out var card))
            {
                return 0;
            }

            if (card.PlayerIndex == Card.PlayerIndex)
            {
                await card.Effect.Boost(12, Card);
            }
            else
            {
                await card.Effect.Damage(12, Card);
            }

            return 0;
        }
    }
}