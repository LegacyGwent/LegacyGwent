using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13029")]//阻魔金炸弹
    public class DimeritiumBomb : CardEffect
    {//重置单排上所有的受增益单位。
        public DimeritiumBomb(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var result = await Game.GetSelectRow(Card.PlayerIndex, Card, TurnType.All.GetRow());
            var row = Game.RowToList(Card.PlayerIndex, result).IgnoreConcealAndDead();
            foreach (var card in row)
            {
                if (card.Status.CardRow.IsOnPlace())
                {
                    if (card.Status.HealthStatus > 0)
                        await card.Effect.Reset(Card);
                }
            }
            return 0;
        }
    }
}