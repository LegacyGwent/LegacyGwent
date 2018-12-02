using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12009")]//先祖麦酒
	public class AleOfTheAncestors : CardEffect
	{//在所在排洒下“黄金酒沫”。
		public AleOfTheAncestors(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}