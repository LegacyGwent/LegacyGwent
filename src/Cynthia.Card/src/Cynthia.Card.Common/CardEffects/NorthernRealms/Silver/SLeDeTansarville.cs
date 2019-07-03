using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43014")]//席儿·德·坦沙维耶
	public class SLeDeTansarville : CardEffect
	{//从手牌打出1张铜色/银色“特殊”牌，随后抽1张牌。
		public SLeDeTansarville(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}