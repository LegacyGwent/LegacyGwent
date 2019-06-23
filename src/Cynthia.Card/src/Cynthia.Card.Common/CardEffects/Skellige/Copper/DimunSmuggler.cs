using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64004")]//迪门家族走私贩
	public class DimunSmuggler : CardEffect
	{//将1个铜色单位从己方墓场返回至牌组。
		public DimunSmuggler(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}