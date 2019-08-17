using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54024")] //战舞者
    public class Wardancer : CardEffect, IHandlesEvent<AfterCardSwap>
    {
        //被交换时自动打出至随机排。
        public Wardancer(GameCard card) : base(card)
        {
        }

        public async Task HandleEvent(AfterCardSwap @event)
        {
            if (@event.HandCard != Card)
            {
                return;
            }

            var location = Game.GetRandomCanPlayLocation(Card.PlayerIndex, true);
            await Card.Effect.Summon(location, Card);
        }
    }
}