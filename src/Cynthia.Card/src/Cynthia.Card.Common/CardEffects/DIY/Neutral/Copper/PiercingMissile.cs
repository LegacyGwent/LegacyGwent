using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70156")]//穿甲弹 PiercingMissile
    public class PiercingMissile : CardEffect
    {//
        public PiercingMissile(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            var damage = 7;
            if(target.Status.HealthStatus > 0)
            {
                damage=damage + 3;
            }
            await target.Effect.Damage  (damage, target, BulletType.RedLight, true);
            
            return 0;
        }
    }
}
