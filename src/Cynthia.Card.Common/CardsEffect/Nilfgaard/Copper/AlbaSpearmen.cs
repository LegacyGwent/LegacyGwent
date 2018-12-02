using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("34007")]//阿尔巴师矛兵
	public class AlbaSpearmen : CardEffect
	{//任意方抽牌时获得1点增益。
		public AlbaSpearmen(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}