using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62003")]//维伯约恩
    public class Vabjorn : CardEffect
    {//对1个单位造成2点伤害。若目标已受伤，则将其摧毁。
        public Vabjorn(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            if (target.Status.HealthStatus >= 0)
                {
                    while (target.Status.HealthStatus >= 0)           
                    {
                        await target.Effect.Damage(2, Card, BulletType.FireBall);
                    }
                    return 0;
                }

            else if (target.Status.HealthStatus < 0)
            {
                await target.Effect.ToCemetery(CardBreakEffectType.Scorch);
            }
            return 0;
        }
    }
}