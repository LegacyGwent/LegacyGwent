using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("54001")]//维里赫德旅工兵
	public class VriheddSappers : CardEffect
	{//伏击：2回合后，在回合开始时翻开。
		public VriheddSappers(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}