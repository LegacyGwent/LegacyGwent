using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("34001")]//见习法师
    public class VicovaroNovice : CardEffect
    {
        public VicovaroNovice(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            return 0;
        }
    }
}