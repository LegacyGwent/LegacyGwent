using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12020")]//刚特·欧迪姆
	public class GaunterODimm : CardEffect
	{//发牌员随机创造一张单位牌，你猜测其战力是大于、等于或小于6。如果你猜对了打出该牌。
		public GaunterODimm(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}