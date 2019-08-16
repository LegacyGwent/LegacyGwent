using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("45002")]//左侧翼步兵
    public class LeftFlankInfantry : CardEffect
    {//无特殊能力
        public LeftFlankInfantry(GameCard card) : base(card) { }
    }
}