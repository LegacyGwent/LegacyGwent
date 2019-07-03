using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34007")]//阿尔巴师矛兵
    public class AlbaSpearmen : CardEffect, IHandlesEvent<AfterPlayerDraw>
    {//任意方抽牌时获得1点增益。
        public AlbaSpearmen(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterPlayerDraw @event)
        {
            if (!Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            await Boost(1, Card);
        }
    }
}