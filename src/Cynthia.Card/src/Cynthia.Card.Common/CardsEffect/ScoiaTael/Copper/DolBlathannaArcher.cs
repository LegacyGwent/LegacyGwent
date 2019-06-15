using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54008")] //多尔·布雷坦纳弓箭手
    public class DolBlathannaArcher : CardEffect
    {
        //分别造成3、1点伤害。
        public DolBlathannaArcher(IGwentServerGame game, GameCard card) : base(game, card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var cards = await Game.GetSelectPlaceCards(Card, 2);
            if (cards.Count <= 0) return 0;
            await cards[0].Effect.Damage(3);
            if (cards.Count == 1) return 0;
            await cards[1].Effect.Damage(1);

            return 0;
        }
    }
}