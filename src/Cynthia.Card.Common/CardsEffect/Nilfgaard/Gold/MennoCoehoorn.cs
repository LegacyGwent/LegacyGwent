using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("32008")]//门诺·库霍恩
    public class MennoCoehoorn : CardEffect
    {
        public MennoCoehoorn(IGwentServerGame game, GameCard card) : base(game, card){}
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            return 0;
        }
    }
}