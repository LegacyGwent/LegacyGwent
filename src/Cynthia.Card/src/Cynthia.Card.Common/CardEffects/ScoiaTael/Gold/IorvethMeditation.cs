using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("52012")]//伊欧菲斯：冥想
	public class IorvethMeditation : CardEffect
	{//迫使2个同排的敌军单位互相对决。
		public IorvethMeditation(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{

			bool row1 = Game.PlayersPlace[AnotherPlayer][0].Count > 1;
            bool row2 = Game.PlayersPlace[AnotherPlayer][1].Count > 1;
            bool row3 = Game.PlayersPlace[AnotherPlayer][2].Count > 1;
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

            await first.Effect.Duel(second, Card);

            return 0;
		}
	}
}