using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    //终末之战
    public class RaghNarRoogStatus : RowEffect, IHandlesEvent<AfterTurnStart>
    {
        public override RowStatus StatusType => RowStatus.RaghNarRoog;

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex) return;
            var cards = AliveNotConceal.WhereAllHighest();
            if (cards.Count() == 0) return;
            await cards.Mess(Game.RNG).First().Effect.Damage(2, null, damageType: DamageType.RaghNarRoog);
        }
    }
}