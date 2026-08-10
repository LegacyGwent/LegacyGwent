using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId(CardId.WraithSorcerer)]
    public class WraithSorcerer : CardEffect, IHandlesEvent<OnGameStart>, IHandlesEvent<AfterTurnStart>
    {
        private int _ownerTurnStarts;

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

            _ownerTurnStarts++;
            if (_ownerTurnStarts >= 2)
            {
                _ownerTurnStarts = 0;
                await Card.Effect.Lock(Card);
            }
        }
    }
}
