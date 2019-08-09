using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64017")]//迪门家族轻型长船
    public class DimunLightLongship : CardEffect, IHandlesEvent<AfterTurnOver>
    {//回合结束时，对右侧的单位造成1点伤害，自身获得2点增益。
        public DimunLightLongship(GameCard card) : base(card) { }


        public async Task HandleEvent(AfterTurnOver @event)
        {

            //不是自己的回合结束,或者不在场上,什么事情都不做
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            var taget = Card.GetRangeCard(1, GetRangeType.HollowRight);
            //如果右侧没有单位或者是伏击卡，不触发效果
            if (taget.Count() == 0 || taget.Single().Status.Conceal)
            {
                return;
            }

            await taget.Single().Effect.Damage(1, Card);
            await Card.Effect.Boost(2, Card);
        }


    }
}
