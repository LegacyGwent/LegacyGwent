using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70147")] // 吸血鸟怪 Plumard
    public class Plumard : CardEffect, IHandlesEvent<AfterCardDrain>, IHandlesEvent<AfterCardDestroyed>
    {
        public Plumard(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardDestroyed @event)
        {
            if (!Card.IsDead && (Card.Status.CardRow.IsOnPlace() || Card.Status.CardRow.IsInHand() ||
                Card.Status.CardRow.IsInDeck()) && @event.SourcePlayerIndex == PlayerIndex &&
                @event.SourceType == CardType.Unit && @event.SourceCategories.Contains(Categorie.Vampire))
            {
                await Card.Effect.Boost(1, @event.Source);
            }
        }

        public async Task HandleEvent(AfterCardDrain @event)
        {
            // The legacy event calls the draining unit Target and its victim Source.
            var drainer = @event.Target;
            if (Card.IsAliveOnPlance() && drainer.IsAliveOnPlance() && @event.Source.IsAliveOnPlance() &&
                drainer.Status.CardId != Card.Status.CardId && drainer.PlayerIndex == PlayerIndex &&
                drainer.Status.CardRow == Card.Status.CardRow)
            {
                await Card.Effect.Drain(1, @event.Source);
            }
        }
    }
}
