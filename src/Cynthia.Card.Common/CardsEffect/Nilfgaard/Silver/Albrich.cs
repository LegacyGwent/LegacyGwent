using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("33006")]//亚伯力奇
	public class Albrich : CardEffect
	{//休战：双方各抽1张牌。对方抽到的牌将被揭示。

		public Albrich(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}