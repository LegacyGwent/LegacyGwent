using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("24003")]//远古小雾妖
    public class AncientFoglet : CardEffect, IHandlesEvent<AfterTurnOver>
    {//回合结束时，若场上任意位置有“蔽日浓雾”，则获得1点增益。
        public AncientFoglet(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            var rowindex = new List<int>() { 0, 1, 2 };
            foreach (var i in rowindex)
            {
                if (Game.GameRowEffect[Card.PlayerIndex][i].RowStatus == RowStatus.ImpenetrableFog || Game.GameRowEffect[AnotherPlayer][i].RowStatus == RowStatus.ImpenetrableFog)
                {
                    await Card.Effect.Boost(1, Card);
                    return;
                }


            }
            return;
        }
    }
}