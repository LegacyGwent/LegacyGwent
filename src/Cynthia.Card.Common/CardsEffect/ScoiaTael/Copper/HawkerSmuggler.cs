using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("54010")]//私枭走私者
	public class HawkerSmuggler : CardEffect
	{//每有1个敌军单位被打出，便获得1点增益。
		public HawkerSmuggler(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}