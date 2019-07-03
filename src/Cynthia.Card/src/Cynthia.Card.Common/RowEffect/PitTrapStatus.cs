using System.Threading.Tasks;

namespace Cynthia.Card
{
    //坑陷
    public class PitTrapStatus : RowEffect, IHandlesEvent<SetStatusEffect>, IHandlesEvent<AfterUnitDown>
    {
        public override RowStatus StatusType => RowStatus.PitTrap;

        public async Task HandleEvent(AfterUnitDown @event)
        {
            var target = @event.Target;
            if (target.PlayerIndex == PlayerIndex && target.Status.CardRow == RowPosition)
            {
                await target.Effect.Damage(3, null);
            }
        }

        public async Task HandleEvent(SetStatusEffect @event)
        {
            foreach (var card in RowCards)
            {
                await card.Effect.Damage(3, null);
            }
        }
    }
}