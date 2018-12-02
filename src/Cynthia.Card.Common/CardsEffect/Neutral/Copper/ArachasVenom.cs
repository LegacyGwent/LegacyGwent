using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14004")]//蟹蜘蛛毒液
	public class ArachasVenom : CardEffect
	{//对3个相邻单位造成4点伤害。
		public ArachasVenom(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}