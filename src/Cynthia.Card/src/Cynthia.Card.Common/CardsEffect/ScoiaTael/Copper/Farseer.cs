using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54006")] //先知
    public class Farseer : CardEffect
    {
        //己方回合中，若有除自身外的友军单位或手牌中的单位获得增益，则回合结束时获得2点增益。
        public Farseer(IGwentServerGame game, GameCard card) : base(game, card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            return 0;
        }

        private bool needBoost = false;

        public override async Task OnTurnStart(int playerIndex)
        {
            if (playerIndex == PlayerIndex)
                needBoost = false;
        }

        public override async Task OnCardBoost(GameCard taget, int num, GameCard soure = null)
        {
            if (taget.PlayerIndex == Card.PlayerIndex && Card != taget && Card.Status.CardRow.IsOnPlace())
            {
                needBoost = true;
            }

        }

        public override async Task OnTurnOver(int playerIndex)
        {
            if (playerIndex == PlayerIndex && needBoost)
            {
                await Card.Effect.Boost(boost);
            }

        }

        private const int boost = 2;
    }
}