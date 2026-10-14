using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId(CardId.Egmond)]
    public class Egmond : CardEffect, IHandlesEvent<AfterTurnOver>
    {
        public Egmond(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (Countdown <= 0) await SetCountdown(2);
            await ResolveAbilityOnce();
            return 0;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.Status.CardRow.IsOnPlace()) return;
            await SetCountdown(offset: -1);
            if (Countdown > 0) return;
            await ResolveAbilityOnce();
            await SetCountdown(2);
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
        }
    }
}
