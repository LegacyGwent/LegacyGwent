using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70040")]//恐狼持斧者
    public class TerrorCrewAxeWielder : CardEffect
    {//对一个敌军随机单位造成3点伤害。被丢弃时，再次触发此能力，并将1张恐狼持斧者洗入牌组。

        public TerrorCrewAxeWielder(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            return 0;
        }
    }
}
