using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12017")]//杰洛特：猎魔大师
	public class GeraltProfessional : CardEffect
	{//对1个敌军单位造成4点伤害。若它为“怪兽”单位，则直接将其摧毁。
		public GeraltProfessional(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}