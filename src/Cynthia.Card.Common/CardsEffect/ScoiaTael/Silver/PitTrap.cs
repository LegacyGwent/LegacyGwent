using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("53020")]//陷坑
	public class PitTrap : CardEffect
	{//在对方单排降下灾厄，对所有被影响的单位造成3点伤害。
		public PitTrap(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}