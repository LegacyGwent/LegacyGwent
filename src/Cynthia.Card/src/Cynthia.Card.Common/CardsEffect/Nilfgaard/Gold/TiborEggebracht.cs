using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("32005")]//蒂博尔·艾格布拉杰
	public class TiborEggebracht : CardEffect
	{//休战：获得15点增益，随后对方抽1张铜色牌并揭示它
		public TiborEggebracht(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}