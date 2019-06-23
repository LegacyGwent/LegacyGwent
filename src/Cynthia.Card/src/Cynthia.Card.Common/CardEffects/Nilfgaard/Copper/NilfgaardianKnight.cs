using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("34004")]//尼弗迦德骑士
    public class NilfgaardianKnight : CardEffect
    {
        public NilfgaardianKnight(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            await Card.Effect.Armor(2, Card);
            var target = Game.PlayersHandCard[Card.PlayerIndex].Where(x => !x.Status.IsReveal);
            if (target.Where(x => x.Status.Group == Group.Copper).Count() > 0)
                await target.Where(x => x.Status.Group == Group.Copper).Mess().First().Effect.Reveal(Card);
            else if (target.Where(x => x.Status.Group == Group.Silver).Count() > 0)
                await target.Where(x => x.Status.Group == Group.Silver).Mess().First().Effect.Reveal(Card);
            else if (target.Where(x => x.Status.Group == Group.Gold).Count() > 0)
                await target.Where(x => x.Status.Group == Group.Gold).Mess().First().Effect.Reveal(Card);
            return 0;
        }
    }
}