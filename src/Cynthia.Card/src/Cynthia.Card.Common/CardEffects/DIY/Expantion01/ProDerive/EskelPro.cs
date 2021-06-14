using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130140")]//艾斯卡尔：晋升
    public class EskelPro : CardEffect
    {//召唤“维瑟米尔”和“兰伯特”，并将其变为金色晋升牌。
        public EskelPro(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (true)
            {

            }
            var myDeck = Game.PlayersDeck[PlayerIndex].ToList();
            var lamberts = myDeck.Where(x => x.Status.CardId == CardId.Lambert).ToList();
            var vesemirs = myDeck.Where(x => x.Status.CardId == CardId.Vesemir).ToList();
            foreach (var lambert in lamberts)
            {
                await lambert.Effect.Transform(lambert.CardInfo().CardId+"0", Card, isForce:true);
                await lambert.Effect.Summon(Card.GetLocation(), Card);
            }
            foreach (var vesemir in vesemirs)
            {
                await vesemir.Effect.Transform(vesemir.CardInfo().CardId+"0", Card, isForce:true);
                await vesemir.Effect.Summon(Card.GetLocation(), Card);
            }
            return 0;
        }
    }
}