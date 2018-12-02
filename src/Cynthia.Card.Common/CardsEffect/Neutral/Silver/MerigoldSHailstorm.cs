using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13032")]//特莉丝雹暴术
	public class MerigoldSHailstorm : CardEffect
	{//使单排所有铜色和银色单位的战力减半。
		public MerigoldSHailstorm(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}