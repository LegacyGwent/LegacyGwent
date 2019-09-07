using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54007")] //维里赫德旅骑兵
    public class VriheddDragoon : CardEffect, IHandlesEvent<AfterTurnOver>
    {
        //回合结束时，使手牌中1张随机非间谍单位牌获得1点增益。
        public VriheddDragoon(GameCard card) : base(card)
        {
        }

        private const int boost = 1;

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (!(Card.Status.CardRow.IsOnPlace() && PlayerIndex == @event.PlayerIndex)) return;

            var cards = Game.PlayersHandCard[Card.PlayerIndex];
            var list = cards.Where(x =>
                x.CardInfo().CardType == CardType.Unit && x.CardInfo().CardUseInfo == CardUseInfo.MyRow).ToList();
            if (!list.Any()) return;

            var card = list.Mess().First();

            await card.Effect.Boost(boost, Card);

            return;
        }
    }
}