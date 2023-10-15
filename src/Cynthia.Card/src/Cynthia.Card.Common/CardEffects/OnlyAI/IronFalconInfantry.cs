using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("89005")]//铁隼步兵 IronFalconInfantry
    public class IronFalconInfantry : CardEffect
    {//若具有增益，则使自身增益翻倍。
        public IronFalconInfantry(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var BoostNum = 0;
            if(Card.Status.HealthStatus > 0)
            {
                BoostNum = Card.Status.HealthStatus;
                await Card.Effect.Boost(BoostNum, Card);
            }
            return 0;
        }
    }
}
