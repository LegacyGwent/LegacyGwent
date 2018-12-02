using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14024")]//精良的长矛
	public class MastercraftedSpear : CardEffect
	{//造成等同于己方手牌中1个铜色/银色单位的基础战力的伤害。
		public MastercraftedSpear(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}