using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("61001")]//“瘸子”哈罗德
    public class HaraldTheCripple : CardEffect
    {//对对方同排的1个随机敌军单位造成1点伤害，再重复9次。
        public HaraldTheCripple(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var row = Game.RowToList(AnotherPlayer, Card.Status.CardRow).IgnoreConcealAndDead();
            for (var i = 0; i < 10; i++)
            {
                var card = row.Where(x => x.IsAliveOnPlance()).Mess(Game.RNG).Take(1);
                if (card.Count() > 0)
                    await card.Single().Effect.Damage(1, Card);
            }
            return 0;
        }
    }
}