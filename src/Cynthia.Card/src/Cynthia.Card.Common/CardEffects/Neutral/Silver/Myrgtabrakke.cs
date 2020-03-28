using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13012")]//米尔加塔布雷克
    public class Myrgtabrakke : CardEffect
    {//造成2点伤害，再重复2次。
        public Myrgtabrakke(GameCard card) : base(card) { }

        private int[] damageValue = { 3, 2, 1 };
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            for (var i = 0; i < 3; i++)
            {
                var result = await Game.GetSelectPlaceCards(Card);
                if (result.Count <= 0) return 0;
                await result.Single().Effect.Damage(damageValue[i], Card, BulletType.FireBall);
            }
            return 0;
        }
    }
}