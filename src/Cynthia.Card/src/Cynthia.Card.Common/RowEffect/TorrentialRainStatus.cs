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
            var LowestCard = AliveNotConceal.WhereAllLowest();
            var HighestCard = AliveNotConceal.WhereAllHighest();
            if (AliveNotConceal.Count() == 0) return;
            await LowestCard.Mess(Game.RNG).First().Effect.Damage(1, null, damageType: DamageType.TorrentialRain);
            await HighestCard.Mess(Game.RNG).First().Effect.Damage(1, null, damageType: DamageType.TorrentialRain);
            /*var cards = AliveNotConceal.Mess(Game.RNG).Take(2);
            foreach (var card in cards)
            {
                await card.Effect.Damage(1, null, damageType: DamageType.TorrentialRain);
            }
            */
        }
    }
}