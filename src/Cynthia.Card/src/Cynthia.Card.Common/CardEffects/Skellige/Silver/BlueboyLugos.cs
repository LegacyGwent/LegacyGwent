using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("63004")]//“阿蓝”卢戈
	public class BlueboyLugos : CardEffect
	{//在对方单排生成1只“幽灵鲸”。
		public BlueboyLugos(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}