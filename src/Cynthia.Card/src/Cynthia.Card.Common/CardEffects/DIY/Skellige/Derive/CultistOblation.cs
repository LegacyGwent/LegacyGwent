using System.Threading.Tasks;
namespace Cynthia.Card
{
    [CardEffectId("70090")]
    public class CultistOblation : CardEffect, IHandlesEvent<AfterCardTransform>
    {
        public CultistOblation(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterCardTransform @event)
        {
            if (!Card.IsAliveOnPlance() || Game.GameRound.ToPlayerIndex(Game) != PlayerIndex ||
                @event.PreviousCardId == null || @event.PreviousCardId == @event.TransformedCardId ||
                @event.PreviousType != CardType.Unit || @event.TransformedType != CardType.Unit)
                return;
            await Game.CreateCard(Card.Status.CardId, PlayerIndex, Card.GetLocation(), status =>
            {
                status.CopyCurrentStateFrom(Card.Status);
                status.IsDoomed = true;
            }, source: Card);
            await Card.Effect.Damage(4, Card);
        }
    }
}
