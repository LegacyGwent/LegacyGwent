using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("34007")]//阿尔巴师矛兵
	public class AlbaSpearmen : CardEffect
	{//任意方抽牌时获得1点增益。
		public AlbaSpearmen(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}