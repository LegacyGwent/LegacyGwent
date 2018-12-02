using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14007")]//阻魔金镣铐
	public class DimeritiumShackles : CardEffect
	{//改变1个单位的锁定状态。若为敌军单位，则对它造成4点伤害。
		public DimeritiumShackles(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}