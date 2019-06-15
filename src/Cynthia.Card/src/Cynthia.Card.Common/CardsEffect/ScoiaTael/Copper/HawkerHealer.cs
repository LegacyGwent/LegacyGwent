using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54018")] //私枭治疗者
    public class HawkerHealer : CardEffect
    {
        //使2个友军单位获得3点增益。
        public HawkerHealer(IGwentServerGame game, GameCard card) : base(game, card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var cards = await Game.GetSelectPlaceCards(Card, 2, selectMode: SelectModeType.MyRow);
            if (cards.Count <= 0) return 0;
            foreach (var card in cards)
            {
                await card.Effect.Boost(boost,Card);
            }

            return 0;
        }

        private const int boost = 3;
    }
}