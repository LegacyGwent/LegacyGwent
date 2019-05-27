using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13031")]//行军令
	public class MarchingOrders : CardEffect
	{//使牌组中最弱的铜色/银色单位牌获得2点增益，然后打出它。
		public MarchingOrders(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			var list = Game.PlayersDeck[PlayerIndex]
            .Where(x => ((x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) &&//铜色或者银色
                    x.CardInfo().CardType == CardType.Unit)).WhereAllLowest().ToList();//单位牌
            if (list.Count() == 0) return 0;
            var moveCard = list.Mess().First();
            await moveCard.MoveToCardStayFirst();
            await moveCard.Effect.Boost(2);
			return 1;
		}
	}
}