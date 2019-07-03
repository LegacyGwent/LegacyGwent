using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("14017")]//燕子
	public class Swallow : CardEffect
	{//使1个单位获得10点增益。
		public Swallow(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			var result = await Game.GetSelectPlaceCards(Card);
			if(result.Count<=0) return 0;
			await result.Single().Effect.Boost(10,Card);
			return 0;
		}
	}
}