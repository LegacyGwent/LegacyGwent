using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("54008")]//多尔·布雷坦纳弓箭手
	public class DolBlathannaArcher : CardEffect
	{//分别造成3、1点伤害。
		public DolBlathannaArcher(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}