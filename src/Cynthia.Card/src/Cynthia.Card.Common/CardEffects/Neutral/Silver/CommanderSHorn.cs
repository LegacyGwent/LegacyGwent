using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13027")]//指挥号角
	public class CommanderSHorn : CardEffect
	{//使7个相邻单位获得3点增益。
		public CommanderSHorn(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			var result = await Game.GetSelectPlaceCards(Card,range:3);
			if(result.Count<=0) return 0;
			foreach(var card in result.Single().GetRangeCard(3).ToList())
			{
				if(card.Status.CardRow.IsOnPlace())
					await card.Effect.Boost(3,Card);
			}
			return 0;
		}
	}
}