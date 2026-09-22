using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70159")]//
    public class Crowmother : CardEffect, IHandlesEvent<AfterCardDeath>
    {
        public Crowmother(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var alliedRows = new[] { RowPosition.MyRow1, RowPosition.MyRow2, RowPosition.MyRow3 };
            foreach (var row in alliedRows.Where(x => x != Card.Status.CardRow))
            {
                if (Game.RowToList(PlayerIndex, row).Count() < Game.RowMaxCount)
                {
                    await Game.CreateCard(CardId.Crow, PlayerIndex, new CardLocation(row, int.MaxValue), source: Card);
                }
            }

            var crowsToGenerate = Card.Status.Countdown;
            while (crowsToGenerate-- > 0 &&
                   Game.RowToList(PlayerIndex, Card.Status.CardRow).Count < Game.RowMaxCount)
            {
                await Game.CreateCardAtEnd(CardId.Crow, PlayerIndex, Card.Status.CardRow, source: Card);
            }
            return 0;
        }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target.PlayerIndex != PlayerIndex ||
                @event.Target.Status.CardId != CardId.Crow)
            {
                return;
            }

            await Card.Effect.SetCountdown(offset: 1);
        }
    }
}
