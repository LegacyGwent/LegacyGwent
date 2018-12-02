using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("64034")]//骨制护符
	public class BoneTalisman : CardEffect
	{//择一：复活1个铜色“野兽”或“呓语”单位；或治愈1名友军单位，并使其获得3点强化。
		public BoneTalisman(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}