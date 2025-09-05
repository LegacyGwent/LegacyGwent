using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70181")]//右翼
    public class AlbastraRightWing : CardEffect
    {//2点护甲。
        public AlbastraRightWing(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(2, Card);
            return 0;
        }
    }
}