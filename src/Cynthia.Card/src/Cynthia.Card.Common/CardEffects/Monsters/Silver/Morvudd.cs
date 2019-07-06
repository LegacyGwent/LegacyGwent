using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23010")]//莫伍德
    public class Morvudd : CardEffect
    {//改变1个单位的锁定状态。若目标为敌军，则使其战力减半。
        public Morvudd(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var result = await Game.GetSelectPlaceCards(Card, 1);
            if (result.Count() == 0)
            {
                return 0;
            }
            var card = result.Single();

            await card.Effect.Lock(Card);

            if (card.PlayerIndex != Card.PlayerIndex)
            {
                //伤害向上取整
                var damageValue = (card.Status.Strength + card.Status.HealthStatus + 1) / 2;
                await card.Effect.Damage(damageValue, Card, isPenetrate: true);
            }
            return 0;
        }
    }
}