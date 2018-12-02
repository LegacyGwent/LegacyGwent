using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("54007")]//维里赫德旅骑兵
	public class VriheddDragoon : CardEffect
	{//回合结束时，使手牌中1张随机非间谍单位牌获得1点增益。
		public VriheddDragoon(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}