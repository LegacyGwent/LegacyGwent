using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14015")]//翼手龙鳞甲盾牌
	public class WyvernScaleShield : CardEffect
	{//使1个单位获得等同于手牌中1张铜色/银色单位牌基础战力的增益。
		public WyvernScaleShield(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}