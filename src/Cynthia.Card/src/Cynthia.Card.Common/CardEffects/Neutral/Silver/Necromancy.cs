using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13033")]//死灵术
    public class Necromancy : CardEffect
    {//从双方墓场放逐1个铜色/银色单位，其战力将成为1个友军单位的增益。
        public Necromancy(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            return 0;
        }
    }
}