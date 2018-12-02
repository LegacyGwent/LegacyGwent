using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12005")]//希里：冲刺
	public class CiriDash : CardEffect
	{//被置入墓场时返回牌组，并获得3点强化。
		public CiriDash(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}