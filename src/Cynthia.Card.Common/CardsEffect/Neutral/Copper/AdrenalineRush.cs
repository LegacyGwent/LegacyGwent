using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14002")]//战意激升
	public class AdrenalineRush : CardEffect
	{//改变1个单位的坚韧状态。
		public AdrenalineRush(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}