using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("70001")]//昆恩护盾
	public class Quen : CardEffect
	{//给予手牌中一个铜色单位及其牌组中的同名卡一层护盾，阻挡一次削弱/伤害/重置效果。
		public Quen(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}