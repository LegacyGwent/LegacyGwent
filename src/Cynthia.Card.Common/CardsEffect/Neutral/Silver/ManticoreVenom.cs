using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13030")]//蝎尾狮毒液
	public class ManticoreVenom : CardEffect
	{//造成13点伤害。
		public ManticoreVenom(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}