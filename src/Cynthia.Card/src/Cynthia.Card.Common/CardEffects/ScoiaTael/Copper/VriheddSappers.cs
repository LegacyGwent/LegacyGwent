using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54001")]//维里赫德旅工兵
    public class VriheddSappers : CardEffect//, IHandlesEvent<AfterTurnStart>
    {//伏击：2回合后，在回合开始时翻开。
        public VriheddSappers(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Conceal(Card);
            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace() && Card.Status.Countdown > 0)
            {
                await Card.Effect.SetCountdown(offset: -1);
                if (Card.Effect.Countdown <= 0)
                {
                    await Card.Effect.Ambush();
                }
            }
        }

    }
}