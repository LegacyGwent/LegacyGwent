using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130030")]//莎拉：晋升
    public class SarahPro : CardEffect
    {//交换3张颜色相同的牌。
        public SarahPro(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var deckGroup = Game.PlayersDeck[PlayerIndex].Select(x => x.Status.Group).Distinct().ToArray();
            var selectList = Game.PlayersHandCard[PlayerIndex].Where(x => x.IsAnyGroup(deckGroup)).ToList();
            for(int i = 0; i < 3; i++ )
            {
                deckGroup = Game.PlayersDeck[PlayerIndex].Select(x => x.Status.Group).Distinct().ToArray();
                selectList = Game.PlayersHandCard[PlayerIndex].Where(x => x.IsAnyGroup(deckGroup)).ToList();
                if (!(await Game.GetSelectMenuCards(PlayerIndex, selectList)).TrySingle(out var swapHandCard))
                {
                    continue;
                }
                if (!Game.PlayersDeck[PlayerIndex].Where(x => x.Is(swapHandCard.Status.Group)).TryMessOne(out var swapDeckCard, Game.RNG))
                {
                    continue;
                }
                await swapHandCard.Effect.Swap(swapDeckCard);
            }
            
            return 0;
        }
    }
}