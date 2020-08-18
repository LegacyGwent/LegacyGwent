using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70041")]//鬼针草煎药
    public class BidensBipinnata : CardEffect
    {//伤害最强的敌军单位2点，重复4次。 己方墓地每有一张【鬼针草煎药】或【合欢茎魔药】，则额外重复1次。

        public BidensBipinnata(GameCard card) : base(card) { }

        public override async Task<int> CardUseEffect()
        {
            return 0;
        }
    }
}
