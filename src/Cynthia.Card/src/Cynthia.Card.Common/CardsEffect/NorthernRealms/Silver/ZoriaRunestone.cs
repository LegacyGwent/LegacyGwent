using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43019")]//佐里亚符文石
	public class ZoriaRunestone : CardEffect
	{//创造1张铜色/银色“北方领域”牌。
		public ZoriaRunestone(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}