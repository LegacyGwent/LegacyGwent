using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64029")]//海玫家族女矛手
	public class HeymaeySpearmaiden : CardEffect
	{//对1个友军“机械”或“士兵”单位造成1点伤害，随后从牌组打出1张它的同名牌。
		public HeymaeySpearmaiden(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}