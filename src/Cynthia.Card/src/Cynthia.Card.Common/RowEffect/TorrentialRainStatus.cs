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
            /*原本的效果
            var cards = AliveNotConceal.Mess(Game.RNG).Take(2);
            foreach (var card in cards)
            {
                await card.Effect.Damage(1, null, damageType: DamageType.TorrentialRain);
            }
            */

            if (@event.PlayerIndex != PlayerIndex) return;
            var LowestCard = AliveNotConceal.WhereAllLowest();
            var HighestCard = AliveNotConceal.WhereAllHighest();
            if (AliveNotConceal.Count() == 0) return;
            var exDamage1 = Game.GetPlaceCards(Game.AnotherPlayer(PlayerIndex)).Where(x => x.Status.CardId == CardId.liaogen && !x.Status.IsLock && x.IsAliveOnPlance()).Count();
            var exDamage2 = Game.GetPlaceCards(Game.AnotherPlayer(PlayerIndex)).Where(x => x.Status.CardId == CardId.liaogen && x.Status.HealthStatus < 0  && !x.Status.IsLock && x.IsAliveOnPlance()).Count();
            var exDamage = exDamage1 + exDamage2;
            await LowestCard.Mess(Game.RNG).First().Effect.Damage(1 + exDamage, null, damageType: DamageType.TorrentialRain);
            await HighestCard.Mess(Game.RNG).First().Effect.Damage(1 + exDamage, null, damageType: DamageType.TorrentialRain);
            
        }
    }
}
