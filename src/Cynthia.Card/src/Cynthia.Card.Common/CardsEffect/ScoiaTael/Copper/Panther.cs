using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("54023")]//黑豹
	public class Panther : CardEffect
	{//若对方某排单位少于4个，则对其中1个单位造成7点伤害。
		public Panther(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}