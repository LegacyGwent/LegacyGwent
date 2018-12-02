using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("34026")]//马格尼师
	public class MagneDivision : CardEffect
	{//从牌组随机打出1张铜色“道具”牌。
		public MagneDivision(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}