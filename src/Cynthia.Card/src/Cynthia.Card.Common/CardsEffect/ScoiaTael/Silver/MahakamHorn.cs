using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53021")]//玛哈坎号角
	public class MahakamHorn : CardEffect
	{//择一：创造1张铜色/银色“矮人”牌；或使1个单位获得7点强化。
		public MahakamHorn(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}