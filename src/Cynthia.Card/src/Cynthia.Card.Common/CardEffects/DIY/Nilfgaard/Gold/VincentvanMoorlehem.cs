using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70150")]//文森特·凡·莫拉汉姆 VincentvanMoorlehem
    public class VincentvanMoorlehem : CardEffect
    {//
        public VincentvanMoorlehem(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            int num = target.CardPoint();
            await target.Effect.ToCemetery(CardBreakEffectType.Scorch);
            
            //如果左侧有单位且不是伏击卡
            var Ltaget = target.GetRangeCard(1, GetRangeType.HollowLeft);
            if (Ltaget.Count() != 0 && !Ltaget.Single().Status.Conceal)
            {
                await Ltaget.Single().Effect.Boost(num/2, Card);
            }

            //如果右侧有单位且不是伏击卡
            var Rtaget = target.GetRangeCard(1, GetRangeType.HollowRight);
            if (Rtaget.Count() != 0 && !Rtaget.Single().Status.Conceal)
            {
                await Rtaget.Single().Effect.Boost(num/2, Card);
            }
            return 0;
            
        }
    }
}
