using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13017")]//兰伯特
    public class Lambert : CardEffect
    {//召唤“维瑟米尔”和“艾斯卡尔”。
        public Lambert(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var myDeck = Game.PlayersDeck[PlayerIndex].ToList();
            var eskels = myDeck.Where(x => x.Status.CardId == CardId.Eskel).ToList();
            var vesemirs = myDeck.Where(x => x.Status.CardId == CardId.Vesemir).ToList();
            foreach (var eskel in eskels)
            {
                await eskel.Effect.Summon(Card.GetLocation() + 1, Card);
            }
            foreach (var vesemir in vesemirs)
            {
                await vesemir.Effect.Summon(Card.GetLocation() + 1, Card);
            }
            return 0;
        }
    }
}