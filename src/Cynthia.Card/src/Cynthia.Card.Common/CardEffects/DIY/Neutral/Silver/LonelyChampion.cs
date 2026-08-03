using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70133")]//孤独的勇士 LonelyChampion
    public class LonelyChampion : CardEffect, IHandlesEvent<AfterTurnOver>
    {//回合结束时，若同排没有其他友军单位则获得1点增益；若场上没有其他友军单位则额外获得3点增益。
        public LonelyChampion(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            var otherFriendlyUnits = Game.GetPlaceCards(PlayerIndex).Where(x => x != Card).ToList();
            var sameRowHasOtherUnit = otherFriendlyUnits.Any(x => x.Status.CardRow == Card.Status.CardRow);
            var boost = sameRowHasOtherUnit ? 0 : 1;
            if (otherFriendlyUnits.Count == 0)
            {
                boost += 3;
            }
            if (boost > 0)
            {
                await Card.Effect.Boost(boost, Card);
            }
        }
    }
}
