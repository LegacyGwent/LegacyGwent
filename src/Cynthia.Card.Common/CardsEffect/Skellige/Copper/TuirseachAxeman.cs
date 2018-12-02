using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("64022")]//图尔赛克家族斧兵
	public class TuirseachAxeman : CardEffect
	{//对方同排每有1个敌军单位受到伤害，便获得1点增益。2点护甲。
		public TuirseachAxeman(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}