using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("21005")]//暗影长者
    public class UnseenElder : CardEffect
    {//汲食1个单位一半的战力。
        public UnseenElder(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (!(await Game.GetSelectPlaceCards(Card)).TrySingle(out var target))
            {
                return 0;
            }
            await Card.Effect.Drain((target.CardPoint() + 1) / 2, target);
            return 0;
        }
    }
}
