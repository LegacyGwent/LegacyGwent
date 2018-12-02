using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("22013")]//盖尔
	public class GeEls : CardEffect
	{//检视牌组中1张金色牌和银色牌，打出1张，将另1张置于牌组顶端。
		public GeEls(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}