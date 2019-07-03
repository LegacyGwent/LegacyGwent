using System.Threading.Tasks;

namespace Cynthia.Card
{
    //龙之梦
    public class DragonDreamStatus : RowEffect, IHandlesEvent<BeforeSpecialPlay>
    {
        public override RowStatus StatusType => RowStatus.DragonDream;

        public async Task HandleEvent(BeforeSpecialPlay @event)
        {
            foreach (var card in RowCards)
            {
                await card.Effect.Damage(4, null);
            }
            await Row.SetStatus<NoneStatus>();
        }
    }
}