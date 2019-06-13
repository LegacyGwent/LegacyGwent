using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("54020")]//维里赫德旅军官
	public class VriheddOfficer : CardEffect
	{//交换1张牌，获得等同于它基础战力的增益。
		public VriheddOfficer(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}