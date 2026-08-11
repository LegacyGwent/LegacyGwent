using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70159")]//
    public class Crowmother : CardEffect
    {//
        public Crowmother(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var alliedRows = new[] { RowPosition.MyRow1, RowPosition.MyRow2, RowPosition.MyRow3 };
            foreach (var row in alliedRows.Where(x => x != Card.Status.CardRow))
            {
                if (Game.RowToList(PlayerIndex, row).Count() < Game.RowMaxCount)
                {
                    await Game.CreateCard(CardId.Crow, PlayerIndex, new CardLocation(row, int.MaxValue));
                }
            }

            var cards = Game.PlayersCemetery[PlayerIndex].Where(x => x.Status.CardId == CardId.Crow && x.Status.Strength <= 2).ToList();
            foreach (var card in cards)
            {
                if (Game.RowToList(PlayerIndex, Card.Status.CardRow).Count() >= Game.RowMaxCount)
                {
                    break;
                }

                await card.Effect.Resurrect(new CardLocation(Card.Status.CardRow, int.MaxValue), Card);
            }
            return 0;
        }
    }
}
