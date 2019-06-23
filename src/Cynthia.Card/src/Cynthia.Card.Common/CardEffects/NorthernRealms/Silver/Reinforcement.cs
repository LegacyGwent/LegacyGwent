using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43020")]//增援
	public class Reinforcement : CardEffect
	{//从牌组打出1张铜色/银色“士兵”、“机械”、“军官”或“辅助”单位牌。
		public Reinforcement(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}