using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("43002")]//薇丝
    public class Ves : CardEffect
    {//交换最多2张牌。 
        public Ves(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            for (var i = 0; i < 2; i++)
            {
                var selectList = Game.PlayersHandCard[PlayerIndex].ToList();
                if (!(await Game.GetSelectMenuCards(PlayerIndex, selectList)).TrySingle(out var swapHandCard))
                {
                    return 0;
                }
                if (!Game.PlayersDeck[PlayerIndex].TryMessOne(out var swapDeckCard, Game.RNG))
                {
                    return 0;
                }
                await swapHandCard.Effect.Swap(swapDeckCard);
            }
            return 0;
        }
    }
}