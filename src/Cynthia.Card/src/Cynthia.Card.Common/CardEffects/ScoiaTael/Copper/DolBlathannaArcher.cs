using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54008")] //多尔·布雷坦纳弓箭手
    public class DolBlathannaArcher : CardEffect
    {
        //分别造成3、1点伤害。
        public DolBlathannaArcher(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var card1 = await Game.GetSelectPlaceCards(Card, 1);
            if (!card1.Any()) return 0;
            await card1.Single().Effect.Damage(3, Card);

            var card2 = await Game.GetSelectPlaceCards(Card, 1);
            if (!card2.Any()) return 0;
            await card2.Single().Effect.Damage(1, Card);
            return 0;
        }
    }
}