using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("89008")]//训练新兵 SoldierTrain
    public class SoldierTrain : CardEffect,IHandlesEvent<BeforeCardToCemetery>
    {//
        private int LCount = 0; 
        public SoldierTrain(GameCard card) : base(card) { }

        public async Task HandleEvent(BeforeCardToCemetery @event)
        {
            if(@event.Target.Status.CardId != CardId.LandOfAThousandFables)
            {
                return;
            }

            if(LCount == 0)
            {
                LCount = @event.Target.CardPoint();
                if(LCount > 0)
                {
                    await Card.Effect.Strengthen(LCount, Card);
                }
                return;
            }
            return;
        }
    }
}
