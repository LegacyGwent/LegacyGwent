using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("34010")]//军旗手
	public class StandardBearer : CardEffect
	{//己方每打出1张“士兵”单位牌，便使1个友军单位获得2点增益。
		public StandardBearer(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}