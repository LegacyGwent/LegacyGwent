using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70026")]//贝哈文的伊沃
    public class IvoOfBelhaven : CardEffect
    {//2护甲。回合开始时，若我方总战力大于对方，增益自身2点。遗愿，随机将卡组里一张稀有度最高的猎魔人单位移至卡组顶端。

        public IvoOfBelhaven(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            return 0;
        }
    }
}
