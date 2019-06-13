using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64030")]//海玫家族保卫者
	public class HeymaeyProtector : CardEffect
	{//从牌组打出1张铜色“道具”牌。
		public HeymaeyProtector(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}