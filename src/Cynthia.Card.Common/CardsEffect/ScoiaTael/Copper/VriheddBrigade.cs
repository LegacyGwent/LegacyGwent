using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("54004")]//维里赫德旅
	public class VriheddBrigade : CardEffect
	{//移除所在排的灾厄，并将1个单位移至它所在半场的同排。
		public VriheddBrigade(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}