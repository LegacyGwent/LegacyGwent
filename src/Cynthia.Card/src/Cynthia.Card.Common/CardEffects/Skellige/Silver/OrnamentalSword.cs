using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("63020")]//华美的长剑
	public class OrnamentalSword : CardEffect
	{//创造1个铜色/银色史凯利格“士兵”单位，并使其获得3点强化。
		public OrnamentalSword(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}