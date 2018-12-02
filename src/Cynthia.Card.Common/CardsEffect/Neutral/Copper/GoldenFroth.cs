using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14026")]//黄金酒沫
	public class GoldenFroth : CardEffect
	{//在己方单排洒下恩泽。回合开始时，使2个随机单位获得1点增益。
		public GoldenFroth(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}