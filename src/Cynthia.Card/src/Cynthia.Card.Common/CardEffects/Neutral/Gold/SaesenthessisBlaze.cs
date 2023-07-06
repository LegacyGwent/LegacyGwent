using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12006")]//萨琪亚萨司：龙焰
    public class SaesenthessisBlaze : CardEffect
    {//放逐所有手牌，抽同等数量的牌。
        public SaesenthessisBlaze(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.PlayersHandCard[PlayerIndex].ToList();
            var count = cards.Count();
            foreach(var card in cards)
            {
                await card.Effect.Discard(Card);
            }
            await Game.PlayerDrawCard(PlayerIndex, count);

            return 0;
        }
    }
}
