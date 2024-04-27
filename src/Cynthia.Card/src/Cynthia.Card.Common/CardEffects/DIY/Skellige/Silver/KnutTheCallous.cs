using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("70080")]//“无情者”克努特 KnuttheCallous
    public class KnuttheCallous : CardEffect
    {//
        public KnuttheCallous(GameCard card) : base(card) { }
        private GameCard DTarget = null;
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow && x.Status.Group == Group.Copper && (x.CardInfo().CardType == CardType.Unit));
            if (list.Count() == 0)
            {
                return 0;
            }
            var StrengthList = list.Select(x => (Strength: x.Status.Strength, card: x)).OrderByDescending(x => x.Strength);
            var StrengthMaximun = StrengthList.First().Strength;
            var result = StrengthList.Where(x => x.Strength >= StrengthMaximun).Select(x => x.card);
            if (result.Count() == 0) return 0;
            DTarget = result.First();
            await DTarget.MoveToCardStayFirst();
            return 1;
        }

        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            if (DTarget == null)
            {
                return;
            }
            await DTarget.Effect.Damage(DTarget.CardPoint()/2, Card);
            return;
        }
    }
}


