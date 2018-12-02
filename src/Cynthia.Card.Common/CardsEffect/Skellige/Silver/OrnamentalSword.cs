using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("63020")]//华美的长剑
	public class OrnamentalSword : CardEffect
	{//创造1个铜色/银色史凯利格“士兵”单位，并使其获得3点强化。
		public OrnamentalSword(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}