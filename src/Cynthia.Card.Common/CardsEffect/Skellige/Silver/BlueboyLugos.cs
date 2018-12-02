using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("63004")]//“阿蓝”卢戈
	public class BlueboyLugos : CardEffect
	{//在对方单排生成1只“幽灵鲸”。
		public BlueboyLugos(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}