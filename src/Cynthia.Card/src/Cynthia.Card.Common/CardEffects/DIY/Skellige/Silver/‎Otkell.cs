using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70121")]//欧特克尔 ‎Otkell
    public class Otkell : CardEffect
    {//使对方同排的“倾盆大雨”伤害提高1点,若自身受伤则额外提升1点。
        public Otkell(GameCard card) : base(card) { }
    }
}
