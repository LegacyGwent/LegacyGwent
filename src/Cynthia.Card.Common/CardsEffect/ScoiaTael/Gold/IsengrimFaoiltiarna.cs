using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("52006")]//伊森格林
	public class IsengrimFaoiltiarna : CardEffect
	{//从牌组打出1张铜色/银色伏击牌。
		public IsengrimFaoiltiarna(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}