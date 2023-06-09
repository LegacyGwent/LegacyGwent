using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70089")]//xxxxxxxx
    public class TemerianBesieger : CardEffect
    {//lock a unit, if it’s an ally boost it by 2
        public TemerianBesieger(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var result = await Game.GetSelectPlaceCards(Card, 1, isHasConceal: true);
            if (result.Count() == 0)
            {
                return 0;
            }
            var card = result.Single();

            await card.Effect.Lock(Card);

            if (card.PlayerIndex == Card.PlayerIndex)
            {
                await card.Effect.Boost(2, Card);
            }
            return 0;
        }
    }
}