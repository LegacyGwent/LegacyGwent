using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70062")]//活体盔甲 Living Armor
    public class LivingArmor : CardEffect, IHandlesEvent<BeforeCardDamage>
    {//自身和己方半场同排铜色/银色单位一次最多受到5点伤害。
        public LivingArmor(GameCard card) : base(card) { }
        public async Task HandleEvent(BeforeCardDamage @event)
        {
            //不在场上，返回
            if (!Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            var currentRow = Card.Status.CardRow;
            if (@event.Target.Status.CardRow == currentRow && @event.Target.PlayerIndex == Card.PlayerIndex )
            {
                if(@event.Num>5)
                {
                    @event.Num=5;
                }
            }

            await Task.CompletedTask;
            return;
        }
    }
}