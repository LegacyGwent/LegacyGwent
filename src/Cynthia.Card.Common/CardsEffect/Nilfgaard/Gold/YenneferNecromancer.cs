using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("32012")]//叶奈法：死灵法师
	public class YenneferNecromancer : CardEffect
	{//从对方墓场中复活1张铜色/银色“士兵”牌
		public YenneferNecromancer(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}