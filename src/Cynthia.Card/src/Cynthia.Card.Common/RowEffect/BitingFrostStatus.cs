using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    //冰霜
    public class BitingFrostStatus : RowEffect, IHandlesEvent<AfterTurnStart>
    {
        public override RowStatus StatusType => RowStatus.BitingFrost;

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex) return;
            var cards = AliveNotConceal.WhereAllLowest();
            if (cards.Count() == 0) return;
            //妥协的做法,以后应该会改
            var exDamage = Game.GetPlaceCards(Game.AnotherPlayer(PlayerIndex), RowPosition).Where(x => x.Status.CardId == CardId.WildHuntRider && !x.Status.IsLock && x.IsAliveOnPlance()).Count();
            await cards.Mess(Game.RNG).First().Effect.Damage(2 + exDamage, null, damageType: DamageType.BitingFrost);
        }
    }
}