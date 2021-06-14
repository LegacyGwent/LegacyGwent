using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130130")]//维瑟米尔：晋升
    public class VesemirPro : CardEffect
    {//召唤“艾斯卡尔”和“兰伯特”，并将其变为金色晋升牌。
        public VesemirPro(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var myDeck = Game.PlayersDeck[PlayerIndex].ToList();
            var eskels = myDeck.Where(x => x.Status.CardId == CardId.Eskel).ToList();
            var lamberts = myDeck.Where(x => x.Status.CardId == CardId.Lambert).ToList();
            foreach (var eskel in eskels)
            {
                await eskel.Effect.Transform(eskel.CardInfo().CardId+"0", Card, isForce:true);
                await eskel.Effect.Summon(Card.GetLocation() + 1, Card);
            }
            foreach (var Lambert in lamberts)
            {
                await Lambert.Effect.Transform(Lambert.CardInfo().CardId+"0", Card, isForce:true);
                await Lambert.Effect.Summon(Card.GetLocation(), Card);
            }
            return 0;
        }
    }
}