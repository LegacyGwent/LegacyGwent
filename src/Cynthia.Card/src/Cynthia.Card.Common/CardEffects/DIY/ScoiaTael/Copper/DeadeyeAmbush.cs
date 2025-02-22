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
            var cards = await Game.GetSelectPlaceCards(Card, filter: x => x.PlayerIndex == PlayerIndex);
    
            if (!cards.TrySingle(out var friend))
            {
                return 0;
            }

            var list = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!list.TrySingle(out var enemy))
            {
                return 0;
            }
            
            var row = (friend.Status.CardRow.MyRowToIndex() - 1).IndexToMyRow();
            if (enemy.Status.CardRow != RowPosition.MyRow1)
            {            
                await enemy.Effect.Move(new CardLocation(RowPosition.MyRow1, int.MaxValue), Card);
                await enemy.Effect.Damage(5, Card);
            }
            if (friend.Status.CardRow != RowPosition.MyRow1)
            {            
                await friend.Effect.Move(new CardLocation(row, int.MaxValue), Card);
                await friend.Effect.Boost(5, Card);
            }
            return 0;
        }
    }
}