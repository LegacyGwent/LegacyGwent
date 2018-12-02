using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12023")]//维瑟米尔：导师
	public class VesemirMentor : CardEffect
	{//从牌组打出1张铜色/银色“炼金”牌。
		public VesemirMentor(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}