using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70176")]//杂交兽
    public class Hybrid : CardEffect
    {
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
            }

            var consumeTargets = await Game.GetSelectPlaceCards(
                Card,
                filter: target => target.PlayerIndex == PlayerIndex,
                selectMode: SelectModeType.MyRow);
            if (consumeTargets.TrySingle(out var consumeTarget))
            {
                await Card.Effect.Consume(consumeTarget);
            }

            return 0;
        }
    }
}
