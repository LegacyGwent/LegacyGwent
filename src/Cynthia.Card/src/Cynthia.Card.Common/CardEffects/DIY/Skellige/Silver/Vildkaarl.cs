using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70160")]//维尔卡战士 Vildkaarl
    public class Vildkaarl : CardEffect
    {//
        public Vildkaarl(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var lostPower = 0;
            //如果左侧有单位且不是伏击卡
            var Ltaget = Card.GetRangeCard(1, GetRangeType.HollowLeft);
            if (Ltaget.Count() != 0 && !Ltaget.Single().Status.Conceal)
            {
                var target = Ltaget.Single();
                var before = target.CardPoint();
                await target.Effect.Damage(4, Card);
                lostPower += target.Status.CardRow.IsOnPlace()
                    ? before - target.CardPoint()
                    : before;
            }

            //如果右侧有单位且不是伏击卡
            var Rtaget = Card.GetRangeCard(1, GetRangeType.HollowRight);
            if (Rtaget.Count() != 0 && !Rtaget.Single().Status.Conceal)
            {
                var target = Rtaget.Single();
                var before = target.CardPoint();
                await target.Effect.Damage(4, Card);
                lostPower += target.Status.CardRow.IsOnPlace()
                    ? before - target.CardPoint()
                    : before;
            }

            if (lostPower > 0)
            {
                await Card.Effect.Boost(lostPower, Card);
            }
            return 0;
        }
        
    }
}
