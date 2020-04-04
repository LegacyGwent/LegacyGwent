using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70003")]//布洛克莱昂哨兵
    public class BrokilonSentinel : CardEffect, IHandlesEvent<AfterTurnOver>
    {//己方回合结束时，如果对手同排单位数量正好为4个，则对对方同排所有单位造成1点伤害。
        public BrokilonSentinel(GameCard card) : base(card)
        {
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                var row = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror()).IgnoreConcealAndDead();
                if (row.Count == 4)
                {
                    foreach (var item in row)
                    {
                        await item.Effect.Damage(1, Card);
                    }
                }
            }
        }
    }
}