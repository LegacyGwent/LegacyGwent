using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    //雨
    public class TorrentialRainStatus : RowEffect, IHandlesEvent<AfterTurnStart>
    {
        public override RowStatus StatusType => RowStatus.TorrentialRain;

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex) return;

            var cards = AliveNotConceal.Mess(Game.RNG).Take(2);
            foreach (var card in cards)
            {
                await card.Effect.Damage(1, null);
            }
        }
    }
}