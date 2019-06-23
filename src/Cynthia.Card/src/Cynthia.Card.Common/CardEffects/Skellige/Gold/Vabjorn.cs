using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("62003")]//维伯约恩
	public class Vabjorn : CardEffect
	{//对1个单位造成2点伤害。若目标已受伤，则将其摧毁。
		public Vabjorn(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}