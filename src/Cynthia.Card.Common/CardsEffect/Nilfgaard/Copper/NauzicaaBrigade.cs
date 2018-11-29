using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("34020")]//娜乌西卡旅
    public class NauzicaaBrigade : CardEffect
    {
        public NauzicaaBrigade(IGwentServerGame game, GameCard card) : base(game, card){}
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            return 0;
        }
    }
}