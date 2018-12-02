using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("64031")]//迪门家族海盗船长
	public class DimunPirateCaptain : CardEffect
	{//从牌组打出1个非同名铜色“迪门家族”单位。
		public DimunPirateCaptain(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}