using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62003")]//维伯约恩
    public class Vabjorn : CardEffect
    {//部署：选择一个单位，如果其已受伤则摧毁它；否则对其造成2点伤害直至其受伤并对自身造成基础战力一半的伤害。
        public Vabjorn(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            if (target.Status.HealthStatus >= 0 && target.CardInfo().CardId != "54012")
                {
                    while (target.Status.HealthStatus >= 0)           
                    {
                        await target.Effect.Damage(2, Card, BulletType.FireBall);

                    }
                    var WeakenValue = (Card.Status.Strength + 1) / 2;
                    await Card.Effect.Weaken(WeakenValue, Card);
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