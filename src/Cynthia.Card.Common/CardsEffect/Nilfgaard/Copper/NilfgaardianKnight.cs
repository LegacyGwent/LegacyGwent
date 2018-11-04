using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("64004")]//尼弗迦德骑士
    public class NilfgaardianKnight : CardEffect
    {
        public NilfgaardianKnight(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            await Task.CompletedTask;
            var taget = Game.PlayersHandCard[Card.PlayerIndex].Where(x => !x.Status.IsReveal);
            if (taget.Where(x => x.Status.Group == Group.Copper).Count() > 0)
                await taget.Where(x => x.Status.Group == Group.Copper).Mess().First().Effect.Reveal(Card);
            else if (taget.Where(x => x.Status.Group == Group.Silver).Count() > 0)
                await taget.Where(x => x.Status.Group == Group.Silver).Mess().First().Effect.Reveal(Card);
            else if (taget.Where(x => x.Status.Group == Group.Gold).Count() > 0)
                await taget.Where(x => x.Status.Group == Group.Gold).Mess().First().Effect.Reveal(Card);
            return 0;
        }
    }
}