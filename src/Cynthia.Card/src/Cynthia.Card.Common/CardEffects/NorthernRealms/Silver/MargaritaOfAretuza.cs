using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43011")]//玛格丽塔
    public class MargaritaOfAretuza : CardEffect
    {//重置1个单位，并改变它的锁定状态。
        public MargaritaOfAretuza(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var Target = await Game.GetSelectPlaceCards(Card);
            if (!(Target.Count() == 0))
            {
                await Target.Single().Effect.Reset(Card);
                await Target.Single().Effect.Lock(Card);
            }
            return 0;
        }
    }
}