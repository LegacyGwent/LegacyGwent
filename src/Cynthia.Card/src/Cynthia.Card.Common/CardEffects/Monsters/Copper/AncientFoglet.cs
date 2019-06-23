using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24003")]//远古小雾妖
    public class AncientFoglet : CardEffect
    {//回合结束时，若场上任意位置有“蔽日浓雾”，则获得1点增益。
        public AncientFoglet(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            return 0;
        }
    }
}