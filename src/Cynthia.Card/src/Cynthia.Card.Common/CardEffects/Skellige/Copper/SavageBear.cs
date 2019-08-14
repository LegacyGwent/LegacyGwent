using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64008")]//恶熊
    public class SavageBear : CardEffect, IHandlesEvent<AfterUnitPlay>
    {//对后续打出至对方半场的单位造成1点伤害。
        public SavageBear(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterUnitPlay @event)
        {
            if (Card.PlayerIndex != @event.PlayedCard.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                await @event.PlayedCard.Effect.Damage(1, Card);
            }
        }
    }
}