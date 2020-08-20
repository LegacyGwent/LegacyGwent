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
            //以下代码基于：可以选择我方单位
            //选择一张场上的卡(任意方)
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }

            //如果目标没受伤，结束
            if (target.Status.HealthStatus >= 0)
            {
                await target.Effect.Damage(2, Card);
                return 0;
            }
            await target.Effect.ToCemetery(CardBreakEffectType.Scorch);
            return 0;
        }
    }
}