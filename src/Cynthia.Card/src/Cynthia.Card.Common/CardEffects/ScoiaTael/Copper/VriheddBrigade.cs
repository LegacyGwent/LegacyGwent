using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("54004")]//维里赫德旅
	public class VriheddBrigade : CardEffect
	{//移除所在排的灾厄，并将1个单位移至它所在半场的同排。
		public VriheddBrigade(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}