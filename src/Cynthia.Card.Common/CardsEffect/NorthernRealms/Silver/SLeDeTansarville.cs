using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("43014")]//席儿·德·坦沙维耶
	public class SLeDeTansarville : CardEffect
	{//从手牌打出1张铜色/银色“特殊”牌，随后抽1张牌。
		public SLeDeTansarville(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}