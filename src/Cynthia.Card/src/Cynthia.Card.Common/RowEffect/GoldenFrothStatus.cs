using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    //黄金酒沫
    public class GoldenFrothStatus : RowEffect, IHandlesEvent<AfterTurnStart>
    {
        public override RowStatus StatusType => RowStatus.GoldenFroth;

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex) return;

            var cards = AliveNotConceal.Mess(Game.RNG).Take(2);
            foreach (var card in cards)
            {
                await card.Effect.Boost(1, null);
            }
        }
    }
}