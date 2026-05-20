using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70131")]//考德威尔伯爵 CountCaldwell
    public class CountCaldwell : CardEffect
    {//交换2个敌军单位的基础战力
        public CountCaldwell(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            bool row1 = Game.PlayersPlace[AnotherPlayer][0].IgnoreConcealAndDead().IgnoreImmune().Count > 1;
            bool row2 = Game.PlayersPlace[AnotherPlayer][1].IgnoreConcealAndDead().IgnoreImmune().Count > 1;
            bool row3 = Game.PlayersPlace[AnotherPlayer][2].IgnoreConcealAndDead().IgnoreImmune().Count > 1;
            if (!(row1 || row2 || row3))
            {
                return 0;
            }
            var first = (await Game.GetSelectPlaceCards(Card, filter: x => (
                (row1 && x.Status.CardRow == RowPosition.MyRow1) ||
                (row2 && x.Status.CardRow == RowPosition.MyRow2) ||
                (row3 && x.Status.CardRow == RowPosition.MyRow3)
            ), selectMode: SelectModeType.EnemyRow)).Single();

            var second = (await Game.GetSelectPlaceCards(Card, filter: x => x.PlayerIndex == first.PlayerIndex && x.Status.CardRow == first.Status.CardRow && x != first)).Single();

            int offset = first.Status.Strength - second.Status.Strength;
            if (first.Status.Strength > second.Status.Strength)
            {
                await first.Effect.Weaken(offset, Card);
                await second.Effect.Strengthen(offset, Card);
            }
            if (first.Status.Strength < second.Status.Strength)
            {
                await second.Effect.Weaken(-offset, Card);
                await first.Effect.Strengthen(-offset, Card);
            }
            return 0;
        }
    }
}
