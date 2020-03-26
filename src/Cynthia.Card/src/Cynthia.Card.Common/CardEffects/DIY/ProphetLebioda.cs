using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70007")]//先知雷比欧达

    public class ProphetLebioda : CardEffect, IHandlesEvent<AfterCardBanish>
    {//"被放逐时，使场上所有友军单位获得1点增益。"
        public ProphetLebioda(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterCardBanish @event)
        {
            await Game.Debug("触发雷比欧达效果");

            //如果是自身被放逐
            if (@event.Target == Card)
            {
                var cards = Game.GetAllCard(Card.PlayerIndex)
                    .Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex == Card.PlayerIndex).ToList();
                foreach (var card in cards)
                {
                    await card.Effect.Boost(2, Card);
                }
                return 0;
            }
        }
    }
}
