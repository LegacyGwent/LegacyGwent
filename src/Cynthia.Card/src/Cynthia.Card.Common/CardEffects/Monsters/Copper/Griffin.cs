using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24008")]//狮鹫
	public class Griffin : CardEffect
	{//触发1个铜色友军单位的遗愿效果。
		public Griffin(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}