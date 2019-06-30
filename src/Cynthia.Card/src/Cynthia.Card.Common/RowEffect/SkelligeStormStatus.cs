using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    //史凯利杰风暴
    public class SkelligeStormStatus : RowEffect, IHandlesEvent<AfterTurnStart>
    {
        public override RowStatus StatusType => RowStatus.SkelligeStorm;

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (PlayerIndex != @event.PlayerIndex) return;

            var cards = RowCards.Take(3).ToList();
            if (cards.Count > 0)
            {
                await cards[0].Effect.Damage(2, null);
                if (cards.Count > 1)
                {
                    await cards[1].Effect.Damage(1, null);
                    if (cards.Count > 2)
                    {
                        await cards[2].Effect.Damage(1, null);
                    }
                }
            }
        }
    }
}