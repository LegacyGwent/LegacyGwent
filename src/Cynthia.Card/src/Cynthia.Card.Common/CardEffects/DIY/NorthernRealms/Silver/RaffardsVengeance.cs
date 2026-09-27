using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70212")]
    public class RaffardsVengeance : CardEffect, IHandlesEvent<AfterTurnOver>
    {
        public RaffardsVengeance(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (!Card.Status.CardRow.IsOnPlace() || Card.Status.IsLock) return 0;

            // Siege Master replays Deploy. A replay preserves the existing Countdown,
            // including a value of one, and applies the current Crew steps again.
            if (!Card.Status.IsCountdown)
                await SetCountdown(4);
            await ApplyCrew();
            return 0;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.Status.CardRow.IsOnPlace()) return;

            // Crew does not accelerate ordinary turns. It is re-evaluated only
            // after this turn-end decrement actually fires and resets the shot.
            if (await AdvanceCountdown())
                await ApplyCrew();
        }

        private async Task ApplyCrew()
        {
            var crewCount = Card.GetCrewedCount();
            for (var i = 0; i < crewCount && Card.Status.CardRow.IsOnPlace(); i++)
                await AdvanceCountdown();
        }

        private async Task<bool> AdvanceCountdown()
        {
            await SetCountdown(offset: -1);
            if (Countdown > 0) return false;

            var mages = Game.RowToList(PlayerIndex, Card.Status.CardRow)
                .Count(unit => !unit.Status.Conceal && !unit.IsDead && unit.HasAnyCategorie(Categorie.Mage));
            var selected = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (selected.TrySingle(out var target))
                await target.Effect.Damage(10 + mages, Card);
            // The shot (or empty-target resolution) finishes while Countdown is zero.
            // Then reset before the caller considers the next, already-counted Crew step.
            await SetCountdown(4);
            return true;
        }
    }
}
