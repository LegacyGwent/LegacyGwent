using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("33005")]//艾希蕾
    public class Assire : CardEffect
    {
        public Assire(IGwentServerGame game, GameCard card) : base(game, card){}

        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            return 0;
        }
    }
}