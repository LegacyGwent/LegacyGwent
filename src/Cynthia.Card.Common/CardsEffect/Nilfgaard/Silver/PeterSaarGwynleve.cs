using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("33011")]//彼得·萨尔格温利
	public class PeterSaarGwynleve : CardEffect
	{//重置1个友军单位，使其获得3点强化；或重置1个敌军单位，使其受到3点削弱。
		public PeterSaarGwynleve(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}