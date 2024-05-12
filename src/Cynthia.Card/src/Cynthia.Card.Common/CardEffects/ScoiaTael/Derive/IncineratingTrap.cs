using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("55001")] //焚烧陷阱
    public class IncineratingTrap : CardEffect, IHandlesEvent<AfterTurnOver>
    {
        //对同排除自身外所有单位造成2点伤害，并在回合结束时放逐自身。
        public IncineratingTrap(GameCard card) : base(card)
        {
        }

        private int damage = 2;

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace()) return;
            await Card.Effect.SetCountdown(offset: -1);
            if (Card.Effect.Countdown > 0) return;
            var row = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow).IgnoreConcealAndDead();
            if (Card.CardPoint() < 1) return; // we need to chek if it's still alive after updating the countdown
                foreach (var it in row)
                {
                    if (it != Card)
                    {
                        await it.Effect.Damage(damage, Card);
                    }
                }
            await Card.Effect.Banish();
        }
    }
}