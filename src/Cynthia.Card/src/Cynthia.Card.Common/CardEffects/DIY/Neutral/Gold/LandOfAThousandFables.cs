using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
namespace Cynthia.Card
{
    [CardEffectId("70014")]//童话国度：公正女神
    public class LandOfAThousandFables : CardEffect, IHandlesEvent<AfterPlayerPass>, IHandlesEvent<BeforeRoundStart>
    {//双方都放弃跟牌后，给我方战力增加自身战力的点数，然后放逐自身。
        public LandOfAThousandFables(GameCard card) : base(card) { }
        private int passedCount = 0;
        public async Task HandleEvent(BeforeRoundStart @event)
        {
            await Game.ClientDelay(200);
            await Card.Effect.ToCemetery();
            await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), Card);
            return;
        }
        public async Task HandleEvent(AfterPlayerPass @event)
        {
            await Game.Debug("童话国度复活");
            passedCount += 1;
            if (passedCount == 2)
            {
                await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), Card);
                Card.Status.IsDoomed = true;
                await Game.ClientDelay(200);
            }
            return;
        }
    }
}