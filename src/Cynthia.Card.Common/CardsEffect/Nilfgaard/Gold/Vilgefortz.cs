using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("32006")]//威戈佛特兹
    public class Vilgefortz : CardEffect
    {
        public Vilgefortz(IGwentServerGame game, GameCard card) : base(game, card){}
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            return 0;
        }
    }
}