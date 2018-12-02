using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("34013")]//哨卫
	public class Sentry : CardEffect
	{//使1个“士兵”单位的所有同名牌获得2点增益。

		public Sentry(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}