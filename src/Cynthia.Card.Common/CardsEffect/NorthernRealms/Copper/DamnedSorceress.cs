using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("44025")]//中邪的女术士
	public class DamnedSorceress : CardEffect
	{//若同排有1个“诅咒生物”单位，则造成7点伤害。
		public DamnedSorceress(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}