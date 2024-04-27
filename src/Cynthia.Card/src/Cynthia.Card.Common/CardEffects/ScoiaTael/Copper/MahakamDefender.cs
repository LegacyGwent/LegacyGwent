using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54016")] //玛哈坎捍卫者
    public class MahakamDefender : CardEffect
    {
        //坚韧。
        public MahakamDefender(GameCard card) : base(card){}
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            await Card.Effect.Resilience(Card);
            return 0;
        }
    }
}
