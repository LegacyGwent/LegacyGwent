using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70124")]//林地徘徊者 WoodlandProwlers
    public class WoodlandProwlers : CardEffect
    {//
        public WoodlandProwlers(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            if (!(await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow)).TrySingle(out var target))
            {
                return 0;
            }
            int point = Game.GameRowEffect[AnotherPlayer][target.Status.CardRow.MyRowToIndex()].RowStatus == RowStatus.ImpenetrableFog ? 3 : 1;

            await target.Effect.Weaken(point, Card);

            return 0;
        }
    }
           
}
