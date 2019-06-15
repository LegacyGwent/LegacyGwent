using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54012")] //玛哈坎劫掠者
    public class MahakamMarauder : CardEffect
    {
        //战力改变时（被重置除外），获得2点增益。
        public MahakamMarauder(IGwentServerGame game, GameCard card) : base(game, card)
        {
        }

        public override async Task OnCardBoost(GameCard taget, int num, GameCard soure = null)
        {
            await BoostMyself(taget, soure);
        }

        private async Task BoostMyself(GameCard taget, GameCard soure)
        {
            if (taget == Card && soure != Card && Card.Status.CardRow.IsOnPlace())
                await BoostMyself();
        }

        public override async Task OnCardHurt(GameCard taget, int num, GameCard soure = null)
        {
            await BoostMyself(taget, soure);
        }

        private async Task BoostMyself()
        {
            await Card.Effect.Boost(boost, Card);
        }

        private const int boost = 2;
    }
}