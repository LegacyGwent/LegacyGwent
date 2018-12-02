using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("25008")]//血月
	public class BloodMoon : CardEffect
	{//在对方单排降下灾厄，对该排上所有单位造成2点伤害。
		public BloodMoon(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}