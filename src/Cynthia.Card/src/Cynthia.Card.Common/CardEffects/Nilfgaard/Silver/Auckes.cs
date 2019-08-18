using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33010")]//奥克斯
    public class Auckes : CardEffect
    {//改变2个单位的锁定状态。
        public Auckes(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var result = await Game.GetSelectPlaceCards(Card, 2);
            foreach (var card in result)
            {
                await card.Effect.Lock(Card)
            }
            return 0;
        }
    }
}