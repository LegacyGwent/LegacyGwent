using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70173")]// DeadeyeAmbush
    public class DeadeyeAmbush : CardEffect
    {// select an ally, boost it by 5 and move it to the row below, select an enemy, damage it by 5 and move it to the melee row
        public DeadeyeAmbush(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var cards = await Game.GetSelectPlaceCards(Card, filter: x => ((x.Status.CardRow == RowPosition.MyRow2 || x.Status.CardRow == RowPosition.MyRow3) && x.PlayerIndex == PlayerIndex));
            var list = await Game.GetSelectPlaceCards(Card, filter: x => ((x.Status.CardRow != RowPosition.MyRow1) && x.PlayerIndex != PlayerIndex));
            // select an ally, boost it by 5 and move it to the row below
            if (cards.TrySingle(out var friend))
            {
                var row = (friend.Status.CardRow.MyRowToIndex() - 1).IndexToMyRow();       
                await friend.Effect.Move(new CardLocation(row, int.MaxValue), Card);
                await friend.Effect.Boost(5, Card);
            }
            // select an enemy, damage it by 5 and move it to the melee row
            if (list.TrySingle(out var enemy))
            {
                await enemy.Effect.Move(new CardLocation(RowPosition.MyRow1, int.MaxValue), Card);
                await enemy.Effect.Damage(5, Card);
            }
            return 0;
        }
    }
}