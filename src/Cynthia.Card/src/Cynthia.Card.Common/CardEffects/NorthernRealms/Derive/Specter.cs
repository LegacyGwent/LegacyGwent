using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("45001")]//鬼灵
    public class Specter : CardEffect
    {//无特殊能力
        public Specter(GameCard card) : base(card) { }
    }
}