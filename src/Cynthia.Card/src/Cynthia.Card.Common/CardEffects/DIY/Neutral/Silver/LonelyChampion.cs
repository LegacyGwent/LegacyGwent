using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70133")]//孤独的勇士 LonelyChampion
    public class LonelyChampion : CardEffect, IHandlesEvent<AfterTurnOver>
    {//回合结束时，若场上没有其它友军单位则获得4点增益。
        public LonelyChampion(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            var targets = Game.GetPlaceCards(PlayerIndex).Where(x => x != Card).ToList();
            if (@event.PlayerIndex == PlayerIndex && targets.Count() == 0 && Card.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.Boost(4, Card);
            }
        }
    }
}
