using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("640080")]//恶熊：晋升
    public class SavageBearPro : CardEffect, IHandlesEvent<AfterUnitDown>
    {//对后续出现在对方半场的单位造成1点伤害。
        public SavageBearPro(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (Card.PlayerIndex != @event.Target.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                await @event.Target.Effect.Damage(1, Card);
            }
        }
    }
}