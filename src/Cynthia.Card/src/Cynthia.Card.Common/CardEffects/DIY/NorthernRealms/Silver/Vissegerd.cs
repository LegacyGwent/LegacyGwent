using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70142")]//维赛基德
    public class Vissegerd : CardEffect
    {//造成8点伤害。
        public Vissegerd(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var card1 = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!card1.Any()) return 0;
            await card1.Single().Effect.Damage(8, Card);

            if(Card.Status.HealthStatus > 0)
            {
                var card2 = await Game.GetSelectPlaceCards(Card);
                if (!card2.Any()) return 0;
                await card2.Single().Effect.Damage(Card.Status.HealthStatus, Card);
            }
            return 0;
        }

    }

}
