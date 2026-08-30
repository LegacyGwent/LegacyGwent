using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70038")]//西格瓦尔德
    public class Sigvald : CardEffect, IHandlesEvent<AfterTurnOver>
    {
        public Sigvald(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsInCemetery())
            {
                return;
            }

            await SetCountdown(offset: -1);
            if (Countdown > 0)
            {
                return;
            }

            await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex, false), Card);
            await Card.Effect.Strengthen(1, Card);
            await SetCountdown(value: 2);
        }
    }
}
