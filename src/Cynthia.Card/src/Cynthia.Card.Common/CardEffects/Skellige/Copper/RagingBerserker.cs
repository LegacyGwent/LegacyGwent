using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64013")]//暴怒的狂战士
    public class RagingBerserker : CardEffect, IHandlesEvent<AfterCardHurt>, IHandlesEvent<AfterCardWeaken>, IHandlesEvent<AfterTurnOver>
    {//受伤或被削弱时变为“狂暴的熊”。
        public RagingBerserker(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterCardHurt @event)
        {
            if (@event.Target == Card && Card.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.Transform("65002", Card);
            }
        }
        public async Task HandleEvent(AfterCardWeaken @event)
        {
            if (@event.Target == Card && Card.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.Transform("65002", Card);
            }
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex)
            {
                return;
            }

            if(Card.CardPoint() < Card.Status.Strength)
            {
                await Card.Effect.Transform("65002", Card);
            }
        }
    }
}
