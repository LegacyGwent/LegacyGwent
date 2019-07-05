using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13003")]//莎拉
    public class Sarah : CardEffect
    {//交换1张颜色相同的牌。
        public Sarah(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var deckGroup = Game.PlayersDeck[PlayerIndex].Select(x => x.Status.Group).Distinct().ToArray();
            var selectList = Game.PlayersHandCard[PlayerIndex].Where(x => x.IsAnyGroup(deckGroup)).ToList();
            if (!(await Game.GetSelectMenuCards(PlayerIndex, selectList)).TrySingle(out var swapHandCard))
            {
                return 0;
            }
            if (!Game.PlayersDeck[PlayerIndex].Where(x => x.Is(swapHandCard.Status.Group)).TryMessOne(out var swapDeckCard, Game.RNG))
            {
                return 0;
            }
            await swapHandCard.Effect.Swap(swapDeckCard);
            return 0;
        }
    }
}