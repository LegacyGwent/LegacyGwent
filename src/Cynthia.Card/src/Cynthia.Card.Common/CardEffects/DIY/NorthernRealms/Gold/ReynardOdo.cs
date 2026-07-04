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
    // On turn end, if it has at least 3 Boosts, convert its Boosts into Armor and Boost all allies on the same row without Armor by 2. Repeat up to 2 times.


        public ReynardOdo(GameCard card) : base(card) { }

        public override async Task<int> CardUseEffect()
        {
            await Card.Effect.SetCountdown(value: 3);
            return 0;
        }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace() || Card.Status.Countdown <= 0)
            {
                return;
            }
            if(Card.Status.HealthStatus >= 3)
            {   
                await Card.Effect.Armor(Card.Status.HealthStatus, Card);
                await Card.Effect.Reset(Card);
                var boostlist = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow).IgnoreConcealAndDead().Where(x => x.Status.CardRow.IsOnPlace()).ToList();;
                foreach (var card in boostlist)
                {
                    await card.Effect.Boost(1, Card);
                }
                await Card.Effect.SetCountdown(offset: -1);
                return;
            }
            return;
        }
    }
}
