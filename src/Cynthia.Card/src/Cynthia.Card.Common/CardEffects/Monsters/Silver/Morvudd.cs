using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23010")]//莫伍德
    public class Morvudd : CardEffect
    {//改变1个单位的锁定状态。若目标为敌军，则使其战力减半。
        public Morvudd(GameCard card) : base(card){ }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var result = await Game.GetSelectPlaceCards(Card, 1);
            var card = result.single();
            await card.Effect.Lock(Card);
            if (card.PlayerIndex != PlayerIndex)
            {
                await card.Effect.Damage((target.Status.Strength + target.Status.HealthStatus)/2, Card);
            }
            return 0;
            Card.
        }
    }
}