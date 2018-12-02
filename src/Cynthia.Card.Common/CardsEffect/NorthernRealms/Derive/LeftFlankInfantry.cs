using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("45002")]//左侧翼步兵
	public class LeftFlankInfantry : CardEffect
	{//没有特殊技能。
		public LeftFlankInfantry(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}