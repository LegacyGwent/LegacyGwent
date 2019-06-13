using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13023")]//黑血
	public class BlackBlood : CardEffect
	{//择一：创造1个铜色“食腐生物”或“吸血鬼”单位，并使其获得2点增益；或摧毁1个铜色/银色“食腐生物”或“吸血鬼”单位。
		public BlackBlood(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}