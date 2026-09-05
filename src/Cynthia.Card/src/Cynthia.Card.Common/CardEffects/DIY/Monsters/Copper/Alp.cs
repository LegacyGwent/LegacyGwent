using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70148")] //吸血鬼女 Alp
    public class Alp : CardEffect, IHandlesEvent<AfterTurnOver>
    {
        public Alp(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.IsAliveOnPlance())
            {
                return;
            }
            if (!Game.GetPlaceCards(AnotherPlayer, Card.Status.CardRow)
                .WhereAllHighest().TryMessOne(out var target, Game.RNG))
            {
                return;
            }
            if (target.CardPoint() <= Card.CardPoint())
            {
                await Card.Effect.Drain(1, target);
            }
        }
    }
}
