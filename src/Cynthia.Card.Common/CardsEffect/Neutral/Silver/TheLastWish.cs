using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13037")]//最后的愿望
	public class TheLastWish : CardEffect
	{//随机检视牌组的2张牌，打出1张。
		public TheLastWish(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}