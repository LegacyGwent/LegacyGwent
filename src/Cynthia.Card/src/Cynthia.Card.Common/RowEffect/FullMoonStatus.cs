using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    //满月
    public class FullMoonStatus : RowEffect, IHandlesEvent<AfterTurnStart>
    {
        public override RowStatus StatusType => RowStatus.FullMoon;

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex) return;

            var cards = AliveNotConceal.Where(x => x.Status.Categories.Contains(Categorie.Beast) || x.Status.Categories.Contains(Categorie.Vampire));
            if (cards.Count() == 0) return;
            await cards.Mess(Game.RNG).First().Effect.Boost(2, null);
        }
    }
}