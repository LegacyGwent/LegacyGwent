using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70163")]//雷纳德·奥多 ReynardOdo 
    public class ReynardOdo : CardEffect, IHandlesEvent<AfterTurnOver>
    {//雷纳德·奥多
    // 回合结束时，若至少具有3点增益，将增益转化为护甲，并使同排没有护甲的友军单位获得2点增益。
    // Reynard Odo:
    // On turn end, if it has at least 3 Boosts, convert its Boosts into Armor and Boost all allies on the same row by 2.


        public ReynardOdo(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            if(Card.Status.HealthStatus >= 3)
            {   
                await Card.Effect.Armor(Card.Status.HealthStatus, Card);
                await Card.Effect.Reset(Card);
                var boostlist = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow).IgnoreConcealAndDead().Where(x => x.Status.CardRow.IsOnPlace() && x != Card).ToList();;
                foreach (var card in boostlist)
                {
                    await card.Effect.Boost(2, Card);
                }
                return;
            }
            return;
        }
    }
}
