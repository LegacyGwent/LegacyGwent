using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12040")]//青草试炼
	public class TrialOfTheGrasses : CardEffect
	{//使1个“猎魔人”单位增益至25点战力；或对1个非“猎魔人”单位造成10点伤害，若目标存活，则使其增益至25点战力。
		public TrialOfTheGrasses(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}