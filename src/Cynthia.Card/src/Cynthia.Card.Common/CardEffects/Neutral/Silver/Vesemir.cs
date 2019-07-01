using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13013")]//维瑟米尔
    public class Vesemir : CardEffect
    {//召唤“艾斯卡尔”和“兰伯特”。
        public Vesemir(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var myDeck = Game.PlayersDeck[PlayerIndex].ToList();
            var eskels = myDeck.Where(x => x.Status.CardId == CardId.Eskel).ToList();
            var lamberts = myDeck.Where(x => x.Status.CardId == CardId.Lambert).ToList();
            foreach (var eskel in eskels)
            {
                await eskel.Effect.Summon(Card.GetLocation() + 1, Card);
            }
            foreach (var Lambert in lamberts)
            {
                await Lambert.Effect.Summon(Card.GetLocation(), Card);
            }
            return 0;
        }
    }
}