using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53005")]//丹尼斯·克莱默
	public class DennisCranmer : CardEffect
	{//使位于手牌、牌组和己方半场除自身外所有“矮人”单位获得1点强化。
		public DennisCranmer(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}