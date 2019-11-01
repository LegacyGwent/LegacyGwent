using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54010")] //私枭走私者
    public class HawkerSmuggler : CardEffect, IHandlesEvent<AfterUnitDown>
    {
        //每有1个敌军单位被打出，便获得1点增益。
        public HawkerSmuggler(GameCard card) : base(card)
        {
        }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.Target.PlayerIndex != Card.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.Boost(boost, Card);
            }
        }

        private int boost = 1;
    }
}