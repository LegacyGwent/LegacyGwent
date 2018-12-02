using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("64026")]//德拉蒙家族持盾女卫
	public class DrummondShieldmaid : CardEffect
	{//召唤所有同名牌。
		public DrummondShieldmaid(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}