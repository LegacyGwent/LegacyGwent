using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33020")]//通敌
    public class Treason : CardEffect
    {//迫使2个相邻敌军单位互相对决。
        public Treason(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            bool row1 = Game.PlayersPlace[AnotherPlayer][0].IgnoreConcealAndDead().Count > 1;
            bool row2 = Game.PlayersPlace[AnotherPlayer][1].IgnoreConcealAndDead().Count > 1;
            bool row3 = Game.PlayersPlace[AnotherPlayer][2].IgnoreConcealAndDead().Count > 1;

            if (!(row1 || row2 || row3))
            {
                return 0;
            }

            var first = (await Game.GetSelectPlaceCards(Card, filter: x => (
                (row1 && x.Status.CardRow == RowPosition.MyRow1) ||
                (row2 && x.Status.CardRow == RowPosition.MyRow2) ||
                (row3 && x.Status.CardRow == RowPosition.MyRow3)
            ), selectMode: SelectModeType.EnemyRow)).Single();

            var second = await Game.GetSelectPlaceCards(Card, filter: x => x.PlayerIndex == first.PlayerIndex && x.Status.CardRow == first.Status.CardRow && x != first && (x == first.GetRangeCard(1, GetRangeType.HollowLeft).FirstOrDefault() || x == first.GetRangeCard(1, GetRangeType.HollowRight).FirstOrDefault()));
            if (second.Count() == 0)
            {
                return 0;
            }
            await first.Effect.Duel(second.First(), Card);

            return 0;
        }
    }
}