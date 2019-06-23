using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13014")]//艾斯卡尔
    public class Eskel : CardEffect
    {//召唤“维瑟米尔”和“兰伯特”。
        public Eskel(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            return 0;
        }
    }
}