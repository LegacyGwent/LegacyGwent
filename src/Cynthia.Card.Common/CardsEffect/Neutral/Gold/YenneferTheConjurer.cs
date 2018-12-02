using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12010")]//叶奈法：咒术师
	public class YenneferTheConjurer : CardEffect
	{//回合结束时，对所有最强的敌军单位造成1点伤害。
		public YenneferTheConjurer(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}