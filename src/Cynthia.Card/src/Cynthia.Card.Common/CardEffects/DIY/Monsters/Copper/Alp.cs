using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70148")] //吸血鬼女 Alp
    public class Alp : CardEffect, IHandlesEvent<AfterTurnOver>
    {
        public Alp(GameCard card) : base(card){}
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            var row = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror()).IgnoreConcealAndDead();
            if(row.Count == 0)
            {
                var cards = Game.GetPlaceCards(AnotherPlayer);
                if (cards.Count() == 0)
                {
                    return;
                }
                await Card.Effect.Drain(1, cards.Mess().First());
            }
            return;
        }
    }
}
