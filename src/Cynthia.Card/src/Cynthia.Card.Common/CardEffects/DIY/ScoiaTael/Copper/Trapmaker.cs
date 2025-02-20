using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70175")]//
    public class Trapmaker : CardEffect, IHandlesEvent<AfterCardAmbush>
    {//
        public Trapmaker(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterCardAmbush @event) // when an allied Ambush is activated, Summon a copy of self on the same row. 
        {
            if (@event.Target.PlayerIndex == PlayerIndex && Card.Status.CardRow.IsInDeck())
            {

                var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
                if (list.Count() == 0)
                {
                    return;
                }
                //summon a copy of self on the same row. 
                if (Card == list.Last())
                {
                    await Card.Effect.Summon(@event.Target.GetLocation() + 1, Card);
                }
            }
        }

    }
}
