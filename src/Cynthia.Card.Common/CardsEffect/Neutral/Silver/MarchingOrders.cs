using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13031")]//行军令
	public class MarchingOrders : CardEffect
	{//使牌组中最弱的铜色/银色单位牌获得2点增益，然后打出它。
		public MarchingOrders(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}