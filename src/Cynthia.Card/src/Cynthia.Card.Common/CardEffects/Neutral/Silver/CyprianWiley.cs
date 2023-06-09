using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13009")]//赛浦利安·威利
	public class CyprianWiley : CardEffect
	{//Weaken a unit by 6 or weaken 2 units by 3.
		public CyprianWiley(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{//For some reason when I change the text server breaks, as this game doesn't appear in-game, I left it as-is
			var switchCard = await Card.GetMenuSwitch(
				("寄生之缚", "从牌组打出1张铜色/银色“诅咒生物”牌"),
				("低语", "创造对方初始牌组中1张银色单位牌")
		   );
			if (switchCard == 0)
			{
				var result = await Game.GetSelectPlaceCards(Card);
				if (result.Count <= 0) return 0;
				await result.Single().Effect.Weaken(5, Card);
				return 0;
			}
			if (switchCard == 1)
			{
				var targetlist = await Game.GetSelectPlaceCards(Card, 2, selectMode: SelectModeType.AllRow, filter: x => x.Status.CardRow != Card.Status.CardRow);
				foreach (var target2 in targetlist)
				{
					await target2.Effect.Weaken(3, Card);
				}
				return 0;
			}
			return 0;
		}
	}
}