using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12013")]//兰伯特：剑术大师
	public class LambertSowrdmaster : CardEffect
	{//对一个敌军单位的所有同名牌造成4点伤害。
		public LambertSowrdmaster(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}