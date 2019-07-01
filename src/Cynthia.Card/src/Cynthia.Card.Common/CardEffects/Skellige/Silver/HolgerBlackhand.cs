using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("63011")]//“黑手”霍格
	public class HolgerBlackhand : CardEffect
	{//造成6点伤害。若摧毁目标，则使己方墓场中最强的单位获得3点强化。
		public HolgerBlackhand(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}