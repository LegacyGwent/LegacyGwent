using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70199")]//安德莱格女王 Endrega Queen
    public class EndregaQueen : CardEffect, IHandlesEvent<OnGameStart>, IHandlesEvent<AfterTurnOver>, IHandlesEvent<AfterCardStrengthen>
    {
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

            await SetCountdown(offset: -1);
            if (Countdown > 0)
            {
                return;
            }

            await Game.CreateCard(CardId.EndregaEggs, PlayerIndex, Card.GetLocation());
            await SetCountdown(value: 3);
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
