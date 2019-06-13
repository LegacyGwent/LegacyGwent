using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("32007")]//希拉德
	public class Shilard : CardEffect
	{//休战：若双方牌组都有牌，从双方牌组各抽1张牌。保留1张，将另一张给予对方。
		public Shilard(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}