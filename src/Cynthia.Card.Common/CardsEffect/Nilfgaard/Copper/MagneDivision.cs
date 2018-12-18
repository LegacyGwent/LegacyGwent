using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("34026")]//马格尼师
	public class MagneDivision : CardEffect
	{//从牌组随机打出1张铜色“道具”牌。
		public MagneDivision(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			var list = Game.PlayersDeck[PlayerIndex]
            .Where(x => ((x.Status.Group == Group.Copper) &&//铜色或者银色
                    x.Status.Categories.Contains(Categorie.Item) &&//道具
                    x.CardInfo().CardType == CardType.Special)).ToList();//特殊
            if (list.Count() == 0) return 0;
            var moveCard = list.Mess().First();
            await moveCard.MoveToCardStayFirst();
            return 1;
		}
	}
}