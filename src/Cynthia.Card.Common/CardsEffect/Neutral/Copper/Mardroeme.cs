using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14013")]//致幻菌菇
	public class Mardroeme : CardEffect
	{//择一：重置1个单位，并使其获得3点强化；或重置1个单位，使其受到3点削弱。
		public Mardroeme(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}