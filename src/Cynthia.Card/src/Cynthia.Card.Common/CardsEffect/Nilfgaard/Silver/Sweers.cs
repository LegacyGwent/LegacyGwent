using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("33007")]//斯维尔
	public class Sweers : CardEffect
	{//选择1个敌军单位或对手手牌中1张被揭示的单位牌，将它所有的同名牌从其牌组置入其墓场。
		public Sweers(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			var result = await Game.GetSelectPlaceCards
				(Card,filter:x=>x.Status.CardRow.IsInHand()?x.Status.IsReveal:true,selectMode:SelectModeType.Enemy);
			if(result.Count() == 0) return 0;
			var tab = result.Single().Status.CardId;
			foreach(var card in Game.PlayersDeck[Game.AnotherPlayer(Card.PlayerIndex)].ToList())
			{
				if(card.Status.CardId==tab)
				{
					await card.Effect.ToCemetery();
				}
			}
			return 0;
		}
	}
}