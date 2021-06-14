using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130170")]//兰伯特：晋升
    public class LambertPro : CardEffect
    {//召唤“维瑟米尔”和“艾斯卡尔”，并将其变为金色晋升牌。
        public LambertPro(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var myDeck = Game.PlayersDeck[PlayerIndex].ToList();
            var eskels = myDeck.Where(x => x.Status.CardId == CardId.Eskel).ToList();
            var vesemirs = myDeck.Where(x => x.Status.CardId == CardId.Vesemir).ToList();
            foreach (var eskel in eskels)
            {
                await eskel.Effect.Transform(eskel.CardInfo().CardId+"0", Card, isForce:true);
                await eskel.Effect.Summon(Card.GetLocation() + 1, Card);
            }
            foreach (var vesemir in vesemirs)
            {
                await vesemir.Effect.Transform(vesemir.CardInfo().CardId+"0", Card, isForce:true);
                await vesemir.Effect.Summon(Card.GetLocation() + 1, Card);
            }
            return 0;
        }
    }
}