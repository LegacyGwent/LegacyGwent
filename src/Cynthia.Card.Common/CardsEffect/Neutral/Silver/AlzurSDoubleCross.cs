using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13024")]//阿尔祖召唤术
	public class AlzurSDoubleCross : CardEffect
	{//使牌组中最强的铜色/银色单位牌获得2点增益，然后打出它。
		public AlzurSDoubleCross(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}