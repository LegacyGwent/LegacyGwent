using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12007")]//特莉丝·梅莉葛德
    public class TrissMerigold : CardEffect
    {//造成7点伤害。
        public TrissMerigold(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var result = await Game.GetSelectPlaceCards(Card);
            if (result.Count <= 0) return 0;
            await result.Single().Effect.Damage(7, Card);
            return 0;
        }
    }
}