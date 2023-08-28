using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70043")] //弗妮希尔的突击队
    public class VernossielsCommando : CardEffect, IHandlesEvent<AfterCardSwap>, IHandlesEvent<AfterUnitDown>, IHandlesEvent<AfterCardToCemetery>
    {
        //第2次被交换时自动打出至随机排。
        public VernossielsCommando(GameCard card) : base(card){}
        public async Task HandleEvent(AfterCardSwap @event)
        {
            if (@event.HandCard != Card)
            {
                return;
            }
            await Card.Effect.SetCountdown(offset: -1);
            if (Card.Effect.Countdown <= 0)
            {
                var location = Game.GetRandomCanPlayLocation(Card.PlayerIndex, true);
                await Card.Effect.Summon(location, Card);
            }
        }
        
        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (Card.Status.CardRow.IsOnPlace()||
                Card.Status.CardRow.IsInCemetery()) 
                return;
            
            if (Card.Effect.Countdown == 0 && Card.Status.CardRow.IsInDeck() && @event.Target.Status.CardId == "70044")
            {
                var location = Game.GetRandomCanPlayLocation(Card.PlayerIndex, true);
                await Card.Effect.Summon(location, Card);
            }
        }

        public async Task HandleEvent(AfterCardToCemetery @event)
        {
            if (@event.Target != Card)
            {
                return;
            }
            await @event.Target.Effect.SetCountdown(value: 2);
        }
    }
}
