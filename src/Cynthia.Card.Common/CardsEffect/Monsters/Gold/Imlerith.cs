using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("22005")]//伊勒瑞斯
	public class Imlerith : CardEffect
	{//对1个敌军单位造成4点伤害，若目标位于“刺骨冰霜”之下，则伤害变为8点。
		public Imlerith(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}