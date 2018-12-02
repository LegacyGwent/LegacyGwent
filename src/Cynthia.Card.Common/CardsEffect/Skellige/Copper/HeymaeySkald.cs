using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("64006")]//海玫家族诗人
	public class HeymaeySkald : CardEffect
	{//使所选“家族”的所有友军单位获得1点增益。
		public HeymaeySkald(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}