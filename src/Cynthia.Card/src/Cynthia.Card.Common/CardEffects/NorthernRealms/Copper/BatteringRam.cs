using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44018")]//攻城槌
    public class BatteringRam : CardEffect
    {//对1个敌军单位造成3点伤害。若摧毁目标，则对另一个敌军单位造成3点伤害。 驱动：初始伤害提高1点。
        public BatteringRam(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Damage(3, Card);
            //如果目标没死，结束
            if (!target.IsDead)
            {
                return 0;
            }
            var selectList2 = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target2))
            {
                return 0;
            }
            await target2.Effect.Damage(3, Card);
            return 0;
        }
    }
}