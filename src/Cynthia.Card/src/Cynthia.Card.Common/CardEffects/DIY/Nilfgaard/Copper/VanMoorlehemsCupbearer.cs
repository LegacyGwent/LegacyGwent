using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70152")]//口莫拉汉姆家斟酒侍者 VanMoorlehemsCupbearer
    public class VanMoorlehemsCupbearer : CardEffect, IHandlesEvent<AfterTurnOver>
    {//回合结束时，随机汲取1个具有增益的敌军单位1点战力。
        public VanMoorlehemsCupbearer(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            var cards = Game.GetPlaceCards(AnotherPlayer).FilterCards(filter: x => x.IsAnyGroup(Group.Gold)).ToList();
            if (cards.Count() == 0)
            {
                await Card.Effect.Boost(1, Card);
            }
            return;
        }


    }
}
