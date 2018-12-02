using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("23021")]//怪物巢穴
	public class MonsterNest : CardEffect
	{//生成1个铜色“食腐生物”或“类虫生物”单位，使其获得1点增益。
		public MonsterNest(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}