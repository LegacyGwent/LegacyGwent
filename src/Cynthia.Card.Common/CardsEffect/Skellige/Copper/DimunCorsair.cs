using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("64028")]//迪门家族海贼
	public class DimunCorsair : CardEffect
	{//复活1个铜色“机械”单位。
		public DimunCorsair(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}