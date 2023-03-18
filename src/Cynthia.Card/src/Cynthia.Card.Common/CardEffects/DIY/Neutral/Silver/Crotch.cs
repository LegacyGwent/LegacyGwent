using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70110")]//裤裆
    public class Crotch : CardEffect, IHandlesEvent<AfterTurnOver>
    {//自身战力不低于手牌数时，召唤此单位。
        public Crotch(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsInDeck())
            {
                if (Card.CardPoint() > Game.PlayersHandCard[Card.PlayerIndex].Count())
                {
                    await Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, false), Card);
                }
            }
        }
        
 
    }
}


