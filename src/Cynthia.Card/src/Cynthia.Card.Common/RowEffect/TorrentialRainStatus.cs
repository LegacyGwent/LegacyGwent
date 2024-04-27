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
            if (AliveNotConceal.Count() == 0) return;

            /*原本的效果

            for(var i = 0; i < 2 ;i++)
            {
                var cards = AliveNotConceal.Mess(Game.RNG).Take(1);
                foreach (var card in cards)
                {
                    int exDamage = 0;
                    int exDamage1 = Game.GetPlaceCards(Game.AnotherPlayer(PlayerIndex)).Where(x => x.Status.CardId == CardId.liaogen && !x.Status.IsLock && x.IsAliveOnPlance()).Count();
                    if (exDamage1 != 0){exDamage = exDamage1;}
                    await card.Effect.Damage(1 + exDamage, null, damageType: DamageType.TorrentialRain);
                }
            }
            */
            
            var LowestCard = AliveNotConceal.WhereAllLowest();
            var HighestCard = AliveNotConceal.WhereAllHighest();
            int exDamage = 0;
            int exDamage1 = Game.GetPlaceCards(Game.AnotherPlayer(PlayerIndex)).Where(x => x.Status.CardId == CardId.Otkell && !x.Status.IsLock && x.IsAliveOnPlance()).Count();
            if (exDamage1 != 0){exDamage = exDamage1;}
            await LowestCard.Mess(Game.RNG).First().Effect.Damage(1 + exDamage, null, damageType: DamageType.TorrentialRain);
            await HighestCard.Mess(Game.RNG).First().Effect.Damage(1 + exDamage, null, damageType: DamageType.TorrentialRain);
            
        }
    }
}
