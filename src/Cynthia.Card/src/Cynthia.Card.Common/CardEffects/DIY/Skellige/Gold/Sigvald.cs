using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70038")]//西格瓦尔德
    public class Sigvald : CardEffect, IHandlesEvent<AfterTurnOver>, IHandlesEvent<AfterCardResurrect>
    {
        public Sigvald(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsInCemetery())
            {
                return;
            }

            var location = Game.GetRandomCanPlayLocation(PlayerIndex, false);
            if (location != null)
            {
                await Card.Effect.Resurrect(location, Card);
            }
        }

        public async Task HandleEvent(AfterCardResurrect @event)
        {
            if (@event.Target != Card)
            {
                return;
            }

            await SetCountdown(offset: -1);
            if (Countdown > 0)
            {
                return;
            }

            await SetCountdown(value: 2);
            await Card.Effect.Strengthen(1, Card);
        }
    }
}
