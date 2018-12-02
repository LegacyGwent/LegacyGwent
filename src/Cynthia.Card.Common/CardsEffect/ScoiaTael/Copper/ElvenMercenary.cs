using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("54030")]//精灵佣兵
	public class ElvenMercenary : CardEffect
	{//随机检视牌组中2张铜色“特殊”牌，打出1张。
		public ElvenMercenary(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}