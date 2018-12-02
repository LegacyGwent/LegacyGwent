using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("44007")]//攻城塔
	public class SiegeTower : CardEffect
	{//获得2点增益。 驱动：再次触发此能力。
		public SiegeTower(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}