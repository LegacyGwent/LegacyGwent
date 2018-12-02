using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("23006")]//卢恩
	public class Ruehin : CardEffect
	{//使位于手牌、牌组和己方半场自身外的所有“类虫生物”和“诅咒生物”单位获得1点强化。
		public Ruehin(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}