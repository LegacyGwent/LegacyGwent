using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("54007")]//维里赫德旅骑兵
	public class VriheddDragoon : CardEffect
	{//回合结束时，使手牌中1张随机非间谍单位牌获得1点增益。
		public VriheddDragoon(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}