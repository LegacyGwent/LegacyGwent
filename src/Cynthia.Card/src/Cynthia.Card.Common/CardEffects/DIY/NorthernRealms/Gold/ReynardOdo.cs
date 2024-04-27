using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70163")]//雷纳德·奥多 ReynardOdo 
    public class ReynardOdo : CardEffect, IHandlesEvent<AfterTurnOver>
    {//
        public ReynardOdo(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            if(Card.Status.HealthStatus >= 3)
            {   
                await Card.Effect.Reset(Card);
                var boostlist = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow).IgnoreConcealAndDead().Where(x => x.Status.CardRow.IsOnPlace()).ToList();;
                foreach (var card in boostlist)
                {
                    await card.Effect.Boost(1, Card);
                }
                return;
            }
            return;
        }
    }
}
