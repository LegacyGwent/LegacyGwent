using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("34025")]//帝国魔像
	public class ImperialGolem : CardEffect
	{//每当己方揭示1张对方手牌，便从牌组召唤1张同名牌。
		public ImperialGolem(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}