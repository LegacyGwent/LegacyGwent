using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70140")]//树精林卫 DryadGrovekeeper
    public class DryadGrovekeeper : CardEffect, IHandlesEvent<AfterUnitPlay>
    {//
        public DryadGrovekeeper(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = await Game.GetSelectPlaceCards(Card, 1, selectMode: SelectModeType.MyRow);
            if (cards.Count() == 0)
            {
                return 0;
            }
            await cards.Single().Effect.Boost(7, Card);
            return 0;
        }

        public async Task HandleEvent(AfterUnitPlay @event)
        {
            if (@event.PlayedCard.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace() && @event.PlayedCard != Card)
            {
                if(@event.PlayedCard.CardPoint() < Card.CardPoint())
                {
                    await Card.Effect.Boost(1, Card);
                }
            }
            return;
        }

    }
}
