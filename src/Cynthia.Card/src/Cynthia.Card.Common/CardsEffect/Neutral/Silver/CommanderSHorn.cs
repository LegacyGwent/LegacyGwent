using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13027")]//指挥号角
	public class CommanderSHorn : CardEffect
	{//使5个相邻单位获得3点增益。
		public CommanderSHorn(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			var result = await Game.GetSelectPlaceCards(Card,range:2);
			if(result.Count<=0) return 0;
			foreach(var card in result.Single().GetRangeCard(2).ToList())
			{
				if(card.Status.CardRow.IsOnPlace())
					await card.Effect.Boost(3,Card);
			}
			return 0;
		}
	}
}