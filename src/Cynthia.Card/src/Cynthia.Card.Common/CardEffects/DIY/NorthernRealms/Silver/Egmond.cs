using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId(CardId.Egmond)]
    public class Egmond : CardEffect, IHandlesEvent<AfterCardBoost>, IHandlesEvent<AfterTurnOver>
    {
        private bool _shouldRepeatAtTurnEnd;
        private bool _isResolvingTurnEndRepeat;

        public Egmond(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await ResolveAbilityOnce();
            return 0;
        }

        public Task HandleEvent(AfterCardBoost @event)
        {
            if (@event.Target != Card ||
                _isResolvingTurnEndRepeat ||
                !Card.Status.CardRow.IsOnPlace() ||
                Game.GameRound.ToPlayerIndex(Game) != PlayerIndex)
            {
                return Task.CompletedTask;
            }

            _shouldRepeatAtTurnEnd = true;
            return Task.CompletedTask;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != PlayerIndex)
            {
                return;
            }

            var shouldRepeat = _shouldRepeatAtTurnEnd;
            _shouldRepeatAtTurnEnd = false;
            if (!shouldRepeat || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            _isResolvingTurnEndRepeat = true;
            try
            {
                await ResolveAbilityOnce();
            }
            finally
            {
                // A repeat cannot schedule another repeat for the next turn.
                _isResolvingTurnEndRepeat = false;
                _shouldRepeatAtTurnEnd = false;
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
                ally.Status.HealthStatus -= removedBoost;
                await Game.ShowCardNumberChange(ally, -removedBoost, NumberType.Normal);
                await Game.ShowSetCard(ally);
                await Game.SetPointInfo();
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
                if (!_isResolvingTurnEndRepeat)
                {
                    _shouldRepeatAtTurnEnd = true;
                }
            }
        }
    }
}
