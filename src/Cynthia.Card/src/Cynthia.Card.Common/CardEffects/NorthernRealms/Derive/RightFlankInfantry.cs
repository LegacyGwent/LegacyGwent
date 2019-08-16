using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("45003")]//右侧翼步兵
    public class RightFlankInfantry : CardEffect
    {//无特殊能力
        public RightFlankInfantry(GameCard card) : base(card) { }
    }
}