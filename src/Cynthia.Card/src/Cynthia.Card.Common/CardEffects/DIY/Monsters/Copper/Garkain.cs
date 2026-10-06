using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70132")]//蝠翼脑魔 Garkain
    public class Garkain: CardEffect
    {//使对方同排的随机单位受到1点伤害5次，若目标受伤则变为汲食。
        public Garkain(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            for (var i = 0; i < 5; i++)
            {
                var cards = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror())
                    .IgnoreConcealAndDead()
                    .ToList();
                if (cards.Count() == 0)
                {
                    return 0;
                }
                var target = cards.Mess(RNG).First();
                if (target.Status.HealthStatus < 0)
                {
                    await Card.Effect.Drain(1, target);
                }
                else
                {
                    await target.Effect.Damage(1, Card);
                }
                
            }
            return 0;
        }
    }
}
