using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70042")]//合欢茎魔药
    public class AlbizziaJulibrissin : CardEffect
    {//增益最弱的友军单位2点，重复4次。 己方墓地每有一张“鬼针草煎药”或“合欢茎魔药”，则额外重复1次。

        public AlbizziaJulibrissin(GameCard card) : base(card) { }

        public override async Task<int> CardUseEffect()
        {
            return 0;
        }
    }
}
