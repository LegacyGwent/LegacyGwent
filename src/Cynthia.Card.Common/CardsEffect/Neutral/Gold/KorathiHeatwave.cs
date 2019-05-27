using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12033")]//科拉兹热浪
	public class KorathiHeatwave : CardEffect
	{//灾厄降于对方全场。 回合开始时，对各排最弱的单位造成2点伤害。
		public KorathiHeatwave(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}