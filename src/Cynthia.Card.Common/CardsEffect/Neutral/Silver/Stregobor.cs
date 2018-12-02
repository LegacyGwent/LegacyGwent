using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13007")]//斯崔葛布
	public class Stregobor : CardEffect
	{//休战：双方各抽1张单位牌，将其战力设为1。
		public Stregobor(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}