using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("52009")]//莫丽恩：森林之女
	public class MorennForestChild : CardEffect
	{//伏击：当对方打出下张铜色/银色特殊牌时，翻开并抵消其能力。
		public MorennForestChild(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}