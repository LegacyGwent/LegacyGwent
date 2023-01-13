using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54020")] //维里赫德旅军官
    public class VriheddOfficer : CardEffect
    {
        //交换1张牌，获得等同于它基础战力的增益。
        public VriheddOfficer(GameCard card) : base(card)
        {
        }

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

            await Card.Effect.Boost(swapHandCard.Status.Strength, Card);
            return 0;
        }
    }
}