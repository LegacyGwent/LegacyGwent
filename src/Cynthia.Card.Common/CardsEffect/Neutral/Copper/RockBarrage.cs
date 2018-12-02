using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14023")]//乱石飞舞
	public class RockBarrage : CardEffect
	{//对1个敌军单位造成7点伤害，并将其上移一排。若该排已满，则将其摧毁。
		public RockBarrage(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}