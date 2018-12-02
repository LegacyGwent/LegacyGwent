using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("54011")]//私枭后援者
	public class HawkerSupport : CardEffect
	{//使手牌中1张单位牌获得3点增益。
		public HawkerSupport(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}