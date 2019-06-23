using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64027")]//海玫家族草药医生
	public class HeymaeyHerbalist : CardEffect
	{//从牌组打出1张随机铜色“有机”或灾厄牌。
		public HeymaeyHerbalist(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}