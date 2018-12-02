using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("23011")]//尼斯里拉
	public class Nithral : CardEffect
	{//对1个敌军单位造成6点伤害。手牌中每有1张“狂猎”单位牌，伤害提高1点。
		public Nithral(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}