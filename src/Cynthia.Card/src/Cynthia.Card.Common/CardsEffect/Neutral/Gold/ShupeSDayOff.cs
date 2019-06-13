using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12041")]//店店的大冒险
	public class ShupeSDayOff : CardEffect
	{//若己方起始牌组没有重复牌，则派“店店”去冒险。
		public ShupeSDayOff(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}