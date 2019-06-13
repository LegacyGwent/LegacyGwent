using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24007")]//狼人头领
    public class AlphaWerewolf : CardEffect
    {//接触“满月”效果时，在自身两侧各生成1只“狼”。
        public AlphaWerewolf(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            return 0;
        }
    }
}