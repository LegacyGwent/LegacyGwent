using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70178")]//倒霉乌雷
    public class Ulle : CardEffect, IHandlesEvent<AfterTurnOver>, IHandlesEvent<AfterTurnStart>
    {
        public Ulle(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsInCemetery())
            {
                return;
            }
            var location = Game.GetRandomCanPlayLocation(PlayerIndex, false);
            if (location != null)
            {
                await Card.Effect.Resurrect(location, Card);
            }
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            if (!Game.GetPlaceCards(AnotherPlayer).WhereAllLowest().TryMessOne(out var target, Game.RNG))
            {
                return;
            }
            await Duel(target, Card);
            if (Card.IsDead || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            await Card.Effect.Weaken(1, Card);
            if (Card.IsDead || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            await Card.Effect.Lock(Card);
            return;
        }
    }
}
