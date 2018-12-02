using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13036")]//召唤法阵
	public class SummoningCircle : CardEffect
	{//生成1张上张被打出的铜色/银色非“密探”单位牌的原始同名牌。
		public SummoningCircle(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}