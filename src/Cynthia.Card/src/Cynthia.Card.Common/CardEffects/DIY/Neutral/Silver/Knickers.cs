using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70110")]
    public class Knickers : CardEffect, IHandlesEvent<AfterTurnOver>
    {
        public Knickers(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            // on same player turn
            if (@event.PlayerIndex != PlayerIndex)
            {
                return;
            }

            int handSize = Game.PlayersHandCard[Card.PlayerIndex].Count();

            // closer the hand size is to 0, more the chance to come out of the deck
            int result = Game.RNG.Next(0, handSize);

            if(result == 0)
            {
                // summon knickers
                var cards = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
                foreach (var card in cards)
                {
                    await card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), Card);
                }
            }
        }
    }
}
