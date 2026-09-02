using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70025")]
    public class Syanna : CardEffect, IHandlesEvent<AfterTurnStart>
    {
        public Syanna(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.SetCountdown(2);
            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace()) return;

            await Card.Effect.SetCountdown(offset: -1);
            if (Card.Effect.Countdown > 0) return;

            await Card.Effect.SetCountdown(2);
            var damage = -Card.Status.HealthStatus;
            if (damage <= 0) return;

            var selected = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (selected.TrySingle(out var target))
            {
                await target.Effect.Damage(damage, Card);
            }
        }
    }
}
