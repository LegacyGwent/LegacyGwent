using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("70171")]//GiantsBelt
	public class GiantsBelt : CardEffect
	{//"Boost an ally by its base Power,if it is an Ogroid, strengthen it by 2 before."
		public GiantsBelt(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			var result = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow);
			if(result.Count<=0) return 0;
			if (result.Single().HasAllCategorie(Categorie.Ogroid))
			{
				await result.Single().Effect.Strengthen(2, Card);
			}
			await result.Single().Effect.Boost(result.Single().Status.Strength, Card);
			return 0;
		}
	}
}