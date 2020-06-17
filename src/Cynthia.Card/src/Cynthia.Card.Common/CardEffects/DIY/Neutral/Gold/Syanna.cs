using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70025")]//席安娜
    public class Syanna : CardEffect
    {//力竭。使你的下一张银色/铜色忠诚单位卡额外触发一次部署效果。

        public Syanna(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            return 0;
        }
    }
}
