using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62005")]//海上野猪
    public class WildBoarOfTheSea : CardEffect, IHandlesEvent<AfterTurnOver>
    {//回合结束时，使左侧单位获得1点强化，右侧单位受到1点伤害。5点护甲。
        public WildBoarOfTheSea(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(5, Card);

            return 0;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {   
            //不是自己的回合结束,或者不在场上,什么事情都不做
            if(@event.PlayerIndex!=Card.PlayerIndex||!Card.Status.CardRow.IsOnPlace() )
            {
                return ;
            }

            //如果左侧有单位且不是伏击卡，使其获得1点强化。
            var Ltaget = Card.GetRangeCard(1, GetRangeType.HollowLeft);
            if (Ltaget.Count() != 0 && !Ltaget.Single().Status.Conceal)
            {
                await Ltaget.Single().Effect.Strengthen(1, Card);
            }

            //如果右侧有单位且不是伏击卡，使其受到1点伤害。
            var Rtaget = Card.GetRangeCard(1, GetRangeType.HollowRight);
            if (Rtaget.Count() != 0 && !Rtaget.Single().Status.Conceal)
            {
                await Rtaget.Single().Effect.Damage(1, Card);
            }
            return;
        }

    }
}