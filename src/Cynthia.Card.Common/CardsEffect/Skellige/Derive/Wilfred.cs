using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("65005")]//威尔弗雷德
	public class Wilfred : CardEffect
	{//遗愿：使1个友军随机单位获得3点强化。
		public Wilfred(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}