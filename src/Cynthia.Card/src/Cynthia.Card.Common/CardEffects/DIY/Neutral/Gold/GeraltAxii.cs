using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70027")]//杰洛特：亚克席法印
    public class GeraltAxii : CardEffect
    {//重新打出对方场上的一张银色/铜色忠诚单位卡，随后将其移回对方半场。

        public GeraltAxii(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            return 0;
        }
    }
}
