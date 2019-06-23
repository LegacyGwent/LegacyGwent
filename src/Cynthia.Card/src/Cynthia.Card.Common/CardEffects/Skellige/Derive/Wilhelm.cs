using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("65006")]//威尔海姆
	public class Wilhelm : CardEffect
	{//遗愿：对对方同排所有单位造成1点伤害。
		public Wilhelm(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}