using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54022")] //玛哈坎守卫
    public class MahakamGuard : CardEffect
    {
        //使1个友军单位获得7点增益。
        public MahakamGuard(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var cards = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow);
            if (!cards.Any()) return 0;
            var card = cards.Single();
            await card.Effect.Boost(boost, Card);

            return 0;
        }

        private int boost = 7;
    }
}