using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12025")]//杰洛特：亚登法印
	public class GeraltYrden : CardEffect
	{//重置单排所有单位，并移除它们的状态。
		public GeraltYrden(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}