using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70176")]//杂交兽
    public class Hybrid : CardEffect, IHandlesEvent<AfterTurnStart>
    {
        private bool _consumeRightAtNextTurnStart;

        public Hybrid(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var deathwishTargets = await Game.GetSelectPlaceCards(
                Card,
                filter: target => target.PlayerIndex == PlayerIndex && target.Status.Group == Group.Copper,
                selectMode: SelectModeType.MyRow);
            if (deathwishTargets.TrySingle(out var deathwishTarget) && !deathwishTarget.Status.IsLock)
            {
                await deathwishTarget.Effects.RaiseEvent(
                    new AfterCardDeath(deathwishTarget, deathwishTarget.GetLocation()));
                _consumeRightAtNextTurnStart = true;
            }

            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !_consumeRightAtNextTurnStart)
            {
                return;
            }

            _consumeRightAtNextTurnStart = false;
            if (!Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            var consumeTarget = Card
                .GetRangeCard(1, GetRangeType.HollowRight)
                .FirstOrDefault();
            if (consumeTarget != null && consumeTarget.PlayerIndex == PlayerIndex)
            {
                await Card.Effect.Consume(consumeTarget);
            }
        }
    }
}
