using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64026")]//德拉蒙家族持盾女卫
	public class DrummondShieldmaid : CardEffect
	{//召唤所有同名牌。
		public DrummondShieldmaid(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}