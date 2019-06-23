using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12035")]//复原
    public class Renew : CardEffect, IHandlesEvent<AfterTurnStart>
    {//复活己方1个非领袖金色单位。
        public Renew(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var result = await Game.GetSelectMenuCards(PlayerIndex, Game.GetAllCard(PlayerIndex), 100);
            foreach (var card in result.Reverse())
            {
                if (card.PlayerIndex == PlayerIndex)
                    await card.MoveToCardStayFirst();
                else
                    await card.MoveToCardStayFirst(true);
            }
            return result.Count;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            foreach (var card in Game.PlayersCemetery[PlayerIndex].ToList())
            {
                await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, 0), card);
            }
            if (!Card.Status.CardRow.IsInHand())
            {
                await Game.ShowCardMove(new CardLocation(RowPosition.MyHand, 0), Card);
            }
        }
    }
}