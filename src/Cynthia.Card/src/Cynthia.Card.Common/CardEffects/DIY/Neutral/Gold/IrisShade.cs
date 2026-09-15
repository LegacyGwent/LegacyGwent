using System.Threading.Tasks;

namespace Cynthia.Card
{
	[CardEffectId("70154")]//爱丽丝：庄园幽影
	public class IrisShade : CardEffect, IHandlesEvent<AfterTurnOver>
	{//休战：回合结束时，为双方手牌各添加1张“爱丽丝的同伴”，一共可生效2次。
		public IrisShade(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			await Card.Effect.SetCountdown(2);
			return 0;
		}

		public async Task HandleEvent(AfterTurnOver @event)
		{
			if (@event.PlayerIndex != PlayerIndex ||
				!Card.Status.CardRow.IsOnPlace() ||
				Card.Status.Countdown <= 0 ||
				Game.IsPlayersPass[AnotherPlayer])
			{
				return;
			}

			await Game.CreateCardAtEnd(CardId.IrisCompanions, PlayerIndex, RowPosition.MyHand);
			await Game.CreateCardAtEnd(CardId.IrisCompanions, AnotherPlayer, RowPosition.MyHand);
			await Card.Effect.SetCountdown(offset: -1);
		}
	}
}
