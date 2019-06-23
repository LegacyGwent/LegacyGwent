using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43012")]//家事妖精
	public class Lubberkin : CardEffect
	{//召唤1只“异婴”。
		public Lubberkin(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}