using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13014")]//艾斯卡尔
    public class Eskel : CardEffect
    {//召唤“维瑟米尔”和“兰伯特”。
        public Eskel(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var myDeck = Game.PlayersDeck[PlayerIndex].ToList();
            var lamberts = myDeck.Where(x => x.Status.CardId == CardId.Lambert).ToList();
            var vesemirs = myDeck.Where(x => x.Status.CardId == CardId.Vesemir).ToList();
            foreach (var lambert in lamberts)
            {
                await lambert.Effect.Summon(Card.GetLocation(), Card);
            }
            foreach (var vesemir in vesemirs)
            {
                await vesemir.Effect.Summon(Card.GetLocation(), Card);
            }
            return 0;
        }
    }
}