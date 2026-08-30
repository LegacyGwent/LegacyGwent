using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId(CardId.Egmond)]
    public class Egmond : CardEffect, IHandlesEvent<AfterCardBoost>
    {
        private bool _isResolvingAbility;
        private int _pendingRepeats;

        public Egmond(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await ResolveAbilityChain();
            return 0;
        }

        public async Task HandleEvent(AfterCardBoost @event)
        {
            if (@event.Target != Card ||
                !Card.Status.CardRow.IsOnPlace() ||
                Game.GameRound.ToPlayerIndex(Game) != PlayerIndex)
            {
                return;
            }

            if (_isResolvingAbility)
            {
                // Boost events are dispatched synchronously. Queue the repeat so
                // this ability's own kill reward is honored without recursive calls.
                _pendingRepeats++;
                return;
            }

            await ResolveAbilityChain();
        }

        private async Task ResolveAbilityChain()
        {
            if (_isResolvingAbility || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            _isResolvingAbility = true;
            try
            {
                var repeats = 1;
                while (repeats-- > 0 && Card.Status.CardRow.IsOnPlace())
                {
                    _pendingRepeats = 0;
                    await ResolveAbilityOnce();
                    repeats += _pendingRepeats;
                }
            }
            finally
            {
                _pendingRepeats = 0;
                _isResolvingAbility = false;
            }
        }

        private async Task ResolveAbilityOnce()
        {
            var allies = await Game.GetSelectPlaceCards(
                Card,
                selectMode: SelectModeType.MyRow);
            if (!allies.TrySingle(out var ally))
            {
                return;
            }

            var removedBoost = Math.Max(0, ally.Status.HealthStatus);
            if (removedBoost > 0)
            {
                await ally.Effect.Reset(Card);
            }

            var enemies = await Game.GetSelectPlaceCards(
                Card,
                selectMode: SelectModeType.EnemyRow);
            if (!enemies.TrySingle(out var enemy))
            {
                return;
            }

            await enemy.Effect.Damage(removedBoost, Card);
            // Lethal damage queues the physical move to the cemetery, so the
            // target can still report an on-board row until this task completes.
            if (enemy.IsDead || !enemy.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.Boost(1, Card);
            }
        }
    }
}
