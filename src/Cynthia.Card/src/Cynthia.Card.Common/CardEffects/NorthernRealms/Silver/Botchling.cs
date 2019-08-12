using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43005")]//异婴
    public class Botchling : CardEffect
    {//召唤1只“家事妖精”。
        public Botchling(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var myDeck = Game.PlayersDeck[PlayerIndex].ToList();
            var Lubberkins = myDeck.Where(x => x.Status.CardId == CardId.Lubberkin).ToList();
            foreach (var Lubberkin in Lubberkins)
            {
                await Lubberkin.Effect.Summon(Card.GetLocation() + 1, Card);
            }
            return 0;
        }
    }
}