using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44027")]//蓝衣铁卫突击队
	public class BlueStripesCommando : CardEffect
	{//有战力与自身相同的非同名“泰莫利亚”友军单位被打出时，从牌组召唤1张它的同名牌。
		public BlueStripesCommando(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}