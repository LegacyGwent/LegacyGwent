using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId(CardId.WraithSorcerer)]
    public class WraithSorcerer : CardEffect, IHandlesEvent<OnGameStart>, IHandlesEvent<AfterTurnStart>
    {
        public WraithSorcerer(GameCard card) : base(card) { }

        public async Task HandleEvent(OnGameStart @event)
        {
            await Card.Effect.Lock(Card);
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            await SetCountdown(offset: -1);
            if (Countdown <= 0)
            {
                await SetCountdown(value: 2);
                if (Card.CardPoint() < 3)
                {
                    return;
                }
                await Card.Effect.Lock(Card);
            }
        }
    }
}
