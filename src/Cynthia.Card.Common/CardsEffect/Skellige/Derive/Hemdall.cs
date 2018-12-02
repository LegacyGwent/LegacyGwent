using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("65001")]//汉姆多尔
	public class Hemdall : CardEffect
	{//摧毁场上所有单位，并移除所有恩泽和灾厄。
		public Hemdall(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}