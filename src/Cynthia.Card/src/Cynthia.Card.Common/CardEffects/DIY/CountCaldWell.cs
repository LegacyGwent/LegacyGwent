using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70002")]//考德威尔伯爵
    public class CountCaldWell : CardEffect
    {//择一：从牌库中打出一张战力不高于自身的铜色单位，在回合结束把它送进墓地；或吞噬牌库中一张战力高于自身的铜色单位牌，将它的战力作为自身的增益。
        public CountCaldWell(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            return 0;
        }
    }
}