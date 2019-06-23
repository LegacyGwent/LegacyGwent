using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("23010")]//莫伍德
	public class Morvudd : CardEffect
	{//改变1个单位的锁定状态。若目标为敌军，则使其战力减半。
		public Morvudd(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}