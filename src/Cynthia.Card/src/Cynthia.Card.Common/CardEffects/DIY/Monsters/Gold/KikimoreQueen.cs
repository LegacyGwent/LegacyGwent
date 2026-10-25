using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70213")]
    public class KikimoreQueen : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<TriggerCountdown>
    {
        public KikimoreQueen(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            Card.Status.IsImmue = true;
            await SetCountdown(2);
            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.IsAliveOnPlance()) return;
            await AdvanceCountdown();
        }

        public Task HandleEvent(TriggerCountdown @event) =>
            @event.Target == Card && Card.IsAliveOnPlance() && Card.Status.IsCountdown
                ? AdvanceCountdown() : Task.CompletedTask;

        private async Task AdvanceCountdown()
        {
            await SetCountdown(offset: -1);
            if (Countdown > 0) return;
            await SetCountdown(2);

            // Snapshot before ticking: transformations, deaths and generated units
            // must not change this activation's set of recipients.
            var targets = Game.GetPlaceCards(PlayerIndex)
                .Where(target => target != Card && target.Status.Categories.Contains(Categorie.Insectoid))
                .ToList();
            foreach (var target in targets)
            {
                if (target.PlayerIndex == PlayerIndex && target.IsAliveOnPlance())
                    await Game.SendEvent(new TriggerCountdown(target));
            }
        }
    }
}
