using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24016")]//狂猎战士
	public class WildHuntWarrior : CardEffect
	{//对1个敌军单位造成3点伤害。若目标位于“刺骨冰霜”之下或被摧毁，则获得2点增益。
		public WildHuntWarrior(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}