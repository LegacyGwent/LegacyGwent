using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12006")]//萨琪亚萨司：龙焰
	public class SaesenthessisBlaze : CardEffect
	{//放逐所有手牌，抽同等数量的牌。
		public SaesenthessisBlaze(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}