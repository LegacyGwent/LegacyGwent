using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("34022")]//毒蛇学派猎魔人
	public class ViperWitcher : CardEffect
	{//己方起始牌组中每有1张“炼金”牌，便造成1点伤害。

		public ViperWitcher(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}