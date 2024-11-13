using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("70171")]//GiantsBelt
	public class GiantsBelt : CardEffect
	{//Boost a friendly unit by its basics Power,if it is an Ogroid, then strengthen it by 3
		public GiantsBelt(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			var result = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow);
			if(result.Count<=0) return 0;
			await result.Single().Effect.Boost(result.Single().Status.Strength, Card);
			if (result.Single().HasAllCategorie(Categorie.Ogroid))
			{
				await result.Single().Effect.Strengthen(3, Card);
			}
			return 0;
		}
	}
}