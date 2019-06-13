using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33010")]//奥克斯
    public class Auckes : CardEffect
    {//改变2个单位的锁定状态。
        public Auckes(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var result = await Game.GetSelectPlaceCards(Card, 2);
            result.ForAll(x => x.Effect.Lock(Card));
            return 0;
        }
    }
}