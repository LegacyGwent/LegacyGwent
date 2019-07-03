using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("14018")]//雷霆
	public class Thunderbolt : CardEffect
	{//使3个相邻单位获得3点增益和2点护甲。
		public Thunderbolt(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			var result = await Game.GetSelectPlaceCards(Card,range:1);
			if(result.Count<=0) return 0;
			foreach(var card in result.Single().GetRangeCard(1).ToList())
			{
				if(card.Status.CardRow.IsOnPlace())
				{
					await card.Effect.Boost(3,Card);
					await card.Effect.Armor(2,Card);
				}
			}
			return 0;
		}
	}
}