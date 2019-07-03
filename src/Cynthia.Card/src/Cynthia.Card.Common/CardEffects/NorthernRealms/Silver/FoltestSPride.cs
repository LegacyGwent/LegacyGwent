using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43007")]//弗尔泰斯特之傲
	public class FoltestSPride : CardEffect
	{//对1个敌军单位造成2点伤害，并将其上移一排。 驱动：再次触发此能力。
		public FoltestSPride(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}