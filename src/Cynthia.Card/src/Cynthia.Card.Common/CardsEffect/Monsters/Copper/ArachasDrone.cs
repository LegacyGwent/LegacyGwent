using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24035")]//蟹蜘蛛雄蛛
    public class ArachasDrone : CardEffect
    {//召唤所有同名牌。
        public ArachasDrone(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            return 0;
        }
    }
}