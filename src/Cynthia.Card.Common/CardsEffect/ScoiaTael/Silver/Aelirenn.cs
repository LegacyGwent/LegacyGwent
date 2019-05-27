using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53009")]//爱黎瑞恩
	public class Aelirenn : CardEffect
	{//场上有至少5个“精灵”友军单位时，在回合结束时召唤此单位。
		public Aelirenn(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}