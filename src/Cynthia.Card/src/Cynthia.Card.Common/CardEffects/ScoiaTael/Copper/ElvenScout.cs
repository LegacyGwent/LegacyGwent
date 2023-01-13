using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54002")]//精灵斥候
    public class ElvenScout : CardEffect
    {//交换一张牌。
        public ElvenScout(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = Game.PlayersHandCard[PlayerIndex].ToList();
            if (!(await Game.GetSelectMenuCards(PlayerIndex, selectList)).TrySingle(out var swapHandCard))
            {
                return 0;
            }
            
            // if (!Game.PlayersDeck[PlayerIndex].TryMessOne(out var swapDeckCard, Game.RNG))
            // {
            //     return 0;
            // }
            // await swapHandCard.Effect.Swap(swapDeckCard);

            await swapHandCard.Effect.SwapWithDeck();

            return 0;
        }
    }
}