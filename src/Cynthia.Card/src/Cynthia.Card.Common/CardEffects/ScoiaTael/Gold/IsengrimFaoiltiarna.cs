using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("52006")]//伊森格林
	public class IsengrimFaoiltiarna : CardEffect
	{//从牌组打出1张铜色/银色伏击牌。
		public IsengrimFaoiltiarna(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			//打乱己方卡组,并且选择所有铜色银色伏击牌
            var list = Game.PlayersDeck[Card.PlayerIndex]
            .Where(x => (x.Status.Group == Group.Copper|| x.Status.Group==Group.Silver) && x.Status.Categories.Contains(Categorie.Ambush)).Mess(RNG);
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
            (Card.PlayerIndex, list.ToList(), 1, "选择打出一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0) return 0;
            await result.First().MoveToCardStayFirst();
            return 1;
		}
	}
}