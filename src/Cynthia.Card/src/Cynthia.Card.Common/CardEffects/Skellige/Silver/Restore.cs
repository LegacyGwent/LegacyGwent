using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("63019")]//回复
	public class Restore : CardEffect
	{//将墓场1张铜色/银色“史凯利格”单位牌置入手牌，为其添加佚亡标签，再将其基础战力设为8点，随后打出1张牌。
		public Restore(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}