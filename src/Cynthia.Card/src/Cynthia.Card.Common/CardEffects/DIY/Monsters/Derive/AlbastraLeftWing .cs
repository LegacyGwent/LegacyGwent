using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70182")]//左翼
    public class AlbastraLeftWing : CardEffect
    {//2点护甲。
        public AlbastraLeftWing(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(2, Card);
            return 0;
        }
    }
}