using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14018")]//雷霆
	public class Thunderbolt : CardEffect
	{//使3个相邻单位获得3点增益和2点护甲。
		public Thunderbolt(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}