using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34014")]//阿尔巴师装甲骑兵
    public class AlbaArmoredCavalry : CardEffect, IHandlesEvent<AfterUnitDown>
    {//每有1个友军单位被打出，便获得1点增益。
        public AlbaArmoredCavalry(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.Target == Card||@event.IsMoveInfo.isMove) return;
            if (PlayerIndex == @event.Target.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                await Boost(1, Card);
            }
        }
    }
}