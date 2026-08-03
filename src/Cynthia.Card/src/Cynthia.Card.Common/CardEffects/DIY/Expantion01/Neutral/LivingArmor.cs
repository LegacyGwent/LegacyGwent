using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70062")]//活体盔甲 Living Armor
    public class LivingArmor : CardEffect, IHandlesEvent<BeforeCardDamage>
    {//己方同排单位受到的伤害减半（向上取整）；同排多个同名效果不叠加。
        public LivingArmor(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Task.CompletedTask;
            return 0;
        }
        public async Task HandleEvent(BeforeCardDamage @event)
        {
            //不在场上，返回
            if (!Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            var currentRow = Card.Status.CardRow;
            var activeSource = Game.RowToList(PlayerIndex, currentRow)
                .FirstOrDefault(x => x.Status.CardId == CardId.LivingArmor &&
                    x.IsAliveOnPlance() && !x.Status.IsLock);
            if (activeSource != Card)
            {
                return;
            }

            if (@event.Target.Status.CardRow == currentRow &&
                @event.Target.PlayerIndex == PlayerIndex)
            {
                @event.Num = (@event.Num + 1) / 2;
            }

            await Task.CompletedTask;
            return;
        }
    }
}
