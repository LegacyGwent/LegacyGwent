using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("24036")]//狂猎导航员
	public class WildHuntNavigator : CardEffect
	{//选择1个非“法师”的友军铜色“狂猎”单位，从牌组打出1张它的同名牌。
		public WildHuntNavigator(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}