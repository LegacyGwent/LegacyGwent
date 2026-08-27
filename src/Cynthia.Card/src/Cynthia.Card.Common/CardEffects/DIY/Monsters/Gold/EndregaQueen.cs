using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70199")]//安德莱格女王 Endrega Queen
    public class EndregaQueen : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterCardStrengthen>
    {
        private int _turnCount;

        public EndregaQueen(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            _turnCount++;
            if (_turnCount < 3)
            {
                return;
            }

            _turnCount = 0;
            await Game.CreateCard(CardId.EndregaEggs, PlayerIndex, Card.GetLocation());
        }

        public async Task HandleEvent(AfterCardStrengthen @event)
        {
            if (@event.Target != Card ||
                !Card.Status.CardRow.IsInDeck() ||
                Card.Status.Strength < 10)
            {
                return;
            }

            var location = Game.GetRandomCanPlayLocation(PlayerIndex, true);
            if (location != null)
            {
                await Card.Effect.Summon(location, Card);
            }
        }
    }
}
