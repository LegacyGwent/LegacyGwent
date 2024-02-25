using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("70158")]//柯恩 CoënofPoviss
    public class Coën : CardEffect
    {//对战力高于自身的单位造成4点伤害。
        public CoënofPoviss(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var Damagenum = 5;
            var count = 1;
            
            for(var i = 0;i < count; i++)
            {
                var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
                if (!selectList.TrySingle(out var target))
                {
                    return 0;
                }
                await target.Effect.Damage(Damagenum, Card);
                if (!target.IsAliveOnPlance())
                {
                    count++;
                    Damagenum=Damagenum-1;
                    if(Damagenum == 0)
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }
    }
}

