using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44007")]//攻城塔
	public class SiegeTower : CardEffect
	{//获得2点增益。 驱动：再次触发此能力。
		public SiegeTower(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}