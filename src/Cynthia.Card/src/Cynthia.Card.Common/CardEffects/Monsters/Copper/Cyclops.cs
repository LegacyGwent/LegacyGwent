using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24001")]//独眼巨人
    public class Cyclops : CardEffect
    {//摧毁1个友军单位，对1个敌军单位造成等同于被摧毁友军单位战力的伤害。
        public Cyclops(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //以下代码基于 如果没有敌军单位 我方单位也会被摧毁
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            int damagepoint = target.CardPoint();
            await target.Effect.ToCemetery();
            var damageList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!damageList.TrySingle(out var Dtarget))
            {
                return 0;
            }
            await Dtarget.Effect.Damage(damagepoint, Card);

            return 0;

        }
    }
}