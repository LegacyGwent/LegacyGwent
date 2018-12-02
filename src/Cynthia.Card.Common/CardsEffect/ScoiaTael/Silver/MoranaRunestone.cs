using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("53018")]//莫拉纳符文石
	public class MoranaRunestone : CardEffect
	{//创造1张铜色/银色“松鼠党”牌。
		public MoranaRunestone(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}