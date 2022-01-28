using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70107")]//安德莱格幼虫
    public class EndregaLarva : CardEffect, IHandlesEvent<AfterTurnOver>
    {//2回合后，回合结束时，转化为“安德莱格战士”。
        public EndregaLarva(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {

            await Card.Effect.SetCountdown(value: 2);
            return 0;
        }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.IsAliveOnPlance())
            {
                return;
            }
            if (!Card.Status.IsCountdown)
            {
                return;
            }
            await Card.Effect.SetCountdown(offset: -1);
            if (Card.Effect.Countdown <= 0)
            {
                await Card.Effect.Transform(CardId.EndregaWarrior, Card, isForce: true);
            }

        }
    }
}