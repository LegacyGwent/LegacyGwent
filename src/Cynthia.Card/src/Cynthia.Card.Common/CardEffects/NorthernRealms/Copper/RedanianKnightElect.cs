using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44013")]//瑞达尼亚当选骑士
    public class RedanianKnightElect : CardEffect, IHandlesEvent<AfterTurnOver>
    {//回合结束时，若受护甲保护，则使相邻单位获得1点增益。 2点护甲。
        public RedanianKnightElect(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(2, Card);
            return 0;
        }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || Card.Status.Armor <= 0)
            {
                return;
            }
            //如果左侧有单位且不是伏击卡
            var Ltaget = Card.GetRangeCard(1, GetRangeType.HollowLeft);
            if (Ltaget.Count() != 0 && !Ltaget.Single().Status.Conceal)
            {
                await Ltaget.Single().Effect.Boost(1, Card);
            }

            //如果右侧有单位且不是伏击卡
            var Rtaget = Card.GetRangeCard(1, GetRangeType.HollowRight);
            if (Rtaget.Count() != 0 && !Rtaget.Single().Status.Conceal)
            {
                await Rtaget.Single().Effect.Boost(1, Card);
            }
            return;
        }
    }
}