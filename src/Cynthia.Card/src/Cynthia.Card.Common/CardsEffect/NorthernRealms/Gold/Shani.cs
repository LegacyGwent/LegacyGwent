using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("42009")]//夏妮
	public class Shani : CardEffect
	{//复活1个铜色/银色非“诅咒生物”单位，并使其获得2点护甲。
		public Shani(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}