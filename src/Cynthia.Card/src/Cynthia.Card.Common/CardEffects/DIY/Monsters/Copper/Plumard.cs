using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70147")] //渴血鸟怪 Plumard
    public class Plumard : CardEffect, IHandlesEvent<AfterCardDrain>
    {
        public Plumard(GameCard card) : base(card){}
        public async Task HandleEvent(AfterCardDrain @event)
        {
            if (@event.Target.Status.CardId != Card.Status.CardId && @event.Target.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace() && @event.Target.Status.CardRow == Card.Status.CardRow)
            {
                await Card.Effect.Drain(1, @event.Source);
            }
            
            return;
        }
    }
}
