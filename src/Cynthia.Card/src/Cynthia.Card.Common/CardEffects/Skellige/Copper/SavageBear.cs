using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64008")]//恶熊
    public class SavageBear : CardEffect, IHandlesEvent<AfterUnitDown>
    {//对后续打出至对方半场的单位造成1点伤害。
        public SavageBear(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (Card.PlayerIndex != @event.Target.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                await @event.Target.Effect.Damage(1, Card);
            }
        }
    }
}