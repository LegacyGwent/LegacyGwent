using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    //浓雾
    public class ImpenetrableFogStatus : RowEffect, IHandlesEvent<AfterTurnStart>
    {
        public override RowStatus StatusType => RowStatus.ImpenetrableFog;

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex) return;
            var cards = AliveNotConceal.WhereAllHighest();
            if (cards.Count() == 0) return;
            await cards.Mess(Game.RNG).First().Effect.Damage(2, null);
        }
    }
}