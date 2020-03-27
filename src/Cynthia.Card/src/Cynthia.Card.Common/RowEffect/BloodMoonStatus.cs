using System.Threading.Tasks;

namespace Cynthia.Card
{
    //血月
    public class BloodMoonStatus : RowEffect, IHandlesEvent<SetStatusEffect>, IHandlesEvent<AfterUnitDown>
    {
        public override RowStatus StatusType => RowStatus.BloodMoon;

        public async Task HandleEvent(AfterUnitDown @event)
        {
            var target = @event.Target;
            if (target.PlayerIndex == PlayerIndex && target.Status.CardRow == RowPosition)
            {
                await target.Effect.Damage(2, null);
            }
        }

        public async Task HandleEvent(SetStatusEffect @event)
        {
            foreach (var card in AliveNotConceal)
            {
                await card.Effect.Damage(2, null, damageType: DamageType.BloodMoon);
            }
        }
    }
}