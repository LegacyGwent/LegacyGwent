using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53011")]//托露薇尔
    public class Toruviel : CardEffect, IHandlesEvent<BeforeUnitPlay>, IHandlesEvent<AfterPlayerPass>
    {//伏击：对方放弃跟牌后翻开，使左右各2格内的单位获得2点增益。
        public Toruviel(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterPlayerPass @event)
        {
            if (@event.PlayerIndex == PlayerIndex || !Card.IsAliveOnPlance() || !Card.Status.Conceal)
            {
                return;
            }
            await Card.Effect.Ambush(async () =>
            {
                var cards = Card.GetRangeCard(2, GetRangeType.HollowAll);
                foreach (var card in cards)
                {
                    await card.Effect.Boost(2, Card);
                }
            });
        }
        public async Task HandleEvent(BeforeUnitPlay @event)
        {
            if (@event.PlayedCard != Card)
            {
                return;
            }
            await Card.Effect.PlanceConceal(Card);
            return;
        }
    }
}