using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130120")]//米尔加塔布雷克：晋升
    public class MyrgtabrakkePro : CardEffect
    {//造成 4、3、2、1 点伤害。
        public MyrgtabrakkePro(GameCard card) : base(card) { }

        private int[] damageValue = { 4, 3, 2, 1 };
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            for (var i = 0; i < 4; i++)
            {
                var result = await Game.GetSelectPlaceCards(Card);
                if (result.Count <= 0) return 0;
                await result.Single().Effect.Damage(damageValue[i], Card, BulletType.FireBall);
            }
            return 0;
        }
    }
}