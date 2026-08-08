using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70098")] //维里赫德旅破坏者 VriheddSaboteur
    public class VriheddSaboteur : CardEffect, IHandlesEvent<AfterCardToDeck>
    {
        public VriheddSaboteur(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardToDeck @event)
        {
            if (Card.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.Boost(1, Card);
            }
        }
    }
}
