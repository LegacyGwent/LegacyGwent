using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70114")]//长弓树精
    public class BowDryad : CardEffect, IHandlesEvent<AfterCardMove>,
        IHandlesEvent<AfterTurnOver>,
        IHandlesEvent<AfterCardDeath>, IHandlesEvent<AfterCardUnLock>
    {
        private bool _repeatAtTurnEnd;

        public BowDryad(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            _repeatAtTurnEnd = false;
            await DealDamage();
            return 0;
        }

        public Task HandleEvent(AfterCardMove @event)
        {
            if (Card.Status.CardRow.IsOnPlace() && !Card.IsDead &&
                (int)Game.GameRound == PlayerIndex &&
                @event.Target.Status.CardRow == RowPosition.MyRow2 &&
                (@event.Target == Card || @event.Target.PlayerIndex != PlayerIndex))
            {
                _repeatAtTurnEnd = true;
            }
            return Task.CompletedTask;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            var shouldRepeat = _repeatAtTurnEnd;
            _repeatAtTurnEnd = false;
            if (@event.PlayerIndex == PlayerIndex && shouldRepeat &&
                Card.Status.CardRow.IsOnPlace() && !Card.IsDead)
                await DealDamage();
        }

        public Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target == Card) _repeatAtTurnEnd = false;
            return Task.CompletedTask;
        }

        public Task HandleEvent(AfterCardUnLock @event)
        {
            // Locked cards miss turn-boundary events; never revive an old pending repeat.
            if (@event.Target == Card) _repeatAtTurnEnd = false;
            return Task.CompletedTask;
        }

        private async Task DealDamage()
        {
            var result = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (result.TrySingle(out var target))
            {
                await target.Effect.Damage(3, Card);
            }
        }
    }
}
