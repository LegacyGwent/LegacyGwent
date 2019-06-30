using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    //干旱(热浪)
    public class KorathiHeatwaveStatus : RowEffect, IHandlesEvent<AfterTurnStart>
    {
        public override RowStatus StatusType => RowStatus.KorathiHeatwave;

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex) return;
            var cards = Row.RowCards.WhereAllLowest();
            if (cards.Count() == 0) return;
            await cards.Mess(Game.RNG).First().Effect.Damage(2, null);
        }
    }
}