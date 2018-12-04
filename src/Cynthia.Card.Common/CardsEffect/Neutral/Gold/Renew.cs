using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12035")]//复原
	public class Renew : CardEffect
	{//复活己方1个非领袖金色单位。
		public Renew(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			//var damage = 4+(Game.PlayersCemetery[Card.PlayerIndex]);
			var count = Game.PlayersDeck[Card.PlayerIndex].Count<3?Game.PlayersDeck[Card.PlayerIndex].Count:3;
			var list = Game.PlayersDeck[Card.PlayerIndex].ToList();
			for(var i = 0; i<count; i++)
			{
				await list[i].MoveToCardStayFirst();
			}
			return count;
		}
		public override async Task OnTurnStart(int playerIndex)
		{
			foreach(var card in Game.PlayersCemetery[Card.PlayerIndex])
			{
				await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck,0),card);
			}
			foreach(var card in Game.PlayersHandCard[Card.PlayerIndex])
			{
				if(card.Status.CardId!="14021")
					await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck,0),card);
			}
			if(!Card.Status.CardRow.IsInHand())
			{
				await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck,0),Card);
			}
		}
	}
}