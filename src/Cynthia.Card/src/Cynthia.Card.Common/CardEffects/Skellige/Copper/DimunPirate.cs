using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64002")]//迪门家族海盗
	public class DimunPirate : CardEffect
	{//丢弃牌组中所有同名牌。
		public DimunPirate(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}