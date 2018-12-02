using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
    [CardEffectId("None")]
    public class NoneEffect : CardEffect
    {
        public NoneEffect(IGwentServerGame game, GameCard card) : base(game, card){}
    }
}