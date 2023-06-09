using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("70090")]//xxxxxxxxxxx
	public class VoydanoiShaman : CardEffect
	{// play a bronze hazard from your deck and spawn and play a copy of it or remove all hazards from the opponent’s board and boost self by 6 for each hazard removed
		public VoydanoiShaman(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			var switchCard = await Card.GetMenuSwitch(
					("寄生之缚", "从牌组打出1张铜色/银色“诅咒生物”牌"),
					("低语", "创造对方初始牌组中1张银色单位牌")
			);
			if (switchCard == 0)
			{
				//play a bronze hazard from your deck and spawn and play a copy of it
				var list = Game.PlayersDeck[Card.PlayerIndex]
				.Where(x => (x.Status.Group == Group.Copper) && x.HasAnyCategorie(Categorie.Hazard)).Mess(RNG);
				var result = await Game.GetSelectMenuCards
				(Card.PlayerIndex, list.ToList(), 1, "选择打出一张牌");
				if (result.Count() == 0) return 0;
				var selctCardId = result.First().Status.CardId;

				await result.First().MoveToCardStayFirst();
				await Game.CreateToStayFirst(selctCardId, PlayerIndex);

				return 2;
			}
			else if (switchCard == 1)
			{ //remove all hazards from the opponent’s board and boost self by 6 for each hazard removed
				var tagetRows = Game.GameRowEffect[AnotherPlayer].Indexed()
					.Where(x => x.Value.RowStatus.IsHazard())
					.Select(x => x.Key);
				foreach (var rowIndex in tagetRows)
				{
					await Game.GameRowEffect[AnotherPlayer][rowIndex].SetStatus<NoneStatus>();
					await Card.Effect.Boost(6, Card);
				}
				return 0;

			}
			return 0;
		}
	}
}