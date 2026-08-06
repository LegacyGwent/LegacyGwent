using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("34004")]//尼弗迦德骑士
    public class NilfgaardianKnight : CardEffect
    {
        public NilfgaardianKnight(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            await Card.Effect.Armor(2, Card);
            var target = Game.PlayersHandCard[Card.PlayerIndex]
                .Where(x => !x.Status.IsReveal)
                .OrderBy(x => x.Status.Group)
                .FirstOrDefault();
            if (target != null)
            {
                await target.Effect.Reveal(Card);
            }
            return 0;
        }
    }
}
