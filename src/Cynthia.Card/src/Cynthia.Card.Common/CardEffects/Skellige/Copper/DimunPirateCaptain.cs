using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64031")]//迪门家族海盗船长
	public class DimunPirateCaptain : CardEffect
	{//从牌组打出1个非同名铜色“迪门家族”单位。
		public DimunPirateCaptain(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}