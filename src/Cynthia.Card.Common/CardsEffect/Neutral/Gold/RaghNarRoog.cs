using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12034")]//终末之战
	public class RaghNarRoog : CardEffect
	{//灾厄降于对方全场。回合开始时，对各排最强的单位造成2点伤害。
		public RaghNarRoog(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}