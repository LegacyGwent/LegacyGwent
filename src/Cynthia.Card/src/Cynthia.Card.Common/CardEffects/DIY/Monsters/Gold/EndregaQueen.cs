using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70199")]//安德莱格女王 Endrega Queen
    public class EndregaQueen : CardEffect, IHandlesEvent<OnGameStart>, IHandlesEvent<AfterTurnOver>, IHandlesEvent<AfterCardStrengthen>
    {
        private int _turnCount;

        public EndregaQueen(GameCard card) : base(card) { }

        public async Task HandleEvent(OnGameStart @event)
        {
            if (!Card.Status.CardRow.IsInDeck())
            {
                return;
            }

            await Game.ShowCardMove(
                new CardLocation(RowPosition.MyDeck, Game.PlayersDeck[PlayerIndex].Count),
                Card);
        }

        public async Task HandleEvent(AfterTurnOver @event)
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

            var meleeRow = Game.PlayersPlace[PlayerIndex][0];
            if (meleeRow.Count < Game.RowMaxCount)
            {
                await Card.Effect.Summon(
                    new CardLocation(RowPosition.MyRow1, meleeRow.Count),
                    Card);
            }
        }
    }
}
