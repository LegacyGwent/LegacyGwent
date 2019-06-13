using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("23012")]//蟾蜍王子
	public class ToadPrince : CardEffect
	{//抽1张单位牌，随后吞噬1张手牌中的单位牌，获得等同于其战力的增益。
		public ToadPrince(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}