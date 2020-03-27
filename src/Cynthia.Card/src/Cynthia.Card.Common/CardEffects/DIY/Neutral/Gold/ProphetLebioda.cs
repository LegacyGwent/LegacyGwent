using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70007")]//先知雷比欧达

    public class ProphetLebioda : CardEffect, IHandlesEvent<BeforeCardBanish>
    {//"被放逐时，使场上所有友军单位获得1点增益。"
        public ProphetLebioda(GameCard card) : base(card) { }

        private const int boostPoint = 1;
        public async Task HandleEvent(BeforeCardBanish @event)
        {
            await Game.Debug("触发雷比欧达效果");

            //如果是自身被放逐
            await Game.Debug(@event.Target.Status.Name);
            if (@event.Target == Card)
            {
                var cards = Game.GetPlaceCards(Card.PlayerIndex)
                    .Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex == Card.PlayerIndex).ToList();
                foreach (var card in cards)
                {
                    await Game.Debug(card.Status.Name);
                    await card.Effect.Boost(boostPoint, Card);
                }
                return;
            }
        }
    }
}
