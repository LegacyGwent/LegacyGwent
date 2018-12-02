using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("53007")]//亚尔潘·齐格林
	public class YarpenZigrin : CardEffect
	{//坚韧。 每打出1个友军“矮人”单位，便获得1点增益。
		public YarpenZigrin(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}