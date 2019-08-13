using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("25002")]//蟹蜘蛛幼虫
    public class ArachasHatchling : CardEffect
    {//召唤所有“蟹蜘蛛雄蛛”。
        public ArachasHatchling(GameCard card) : base(card) { }
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == CardId.ArachasDrone).ToList();
            foreach (var card in cards)
            {
                await card.Effect.Summon(Card.GetLocation() + 1, Card);
            }
        }
    }
}