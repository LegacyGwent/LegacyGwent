using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64012")]//图尔赛克家族好斗分子
	public class TuirseachSkirmisher : CardEffect
	{//被复活后获得3点强化。
		public TuirseachSkirmisher(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}