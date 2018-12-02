using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14027")]//农民民兵
	public class PeasantMilitia : CardEffect
	{//在己方单排生成3个“农民”单位。
		public PeasantMilitia(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}