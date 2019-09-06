using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54001")]//维里赫德旅工兵
    public class VriheddSappers : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<BeforeUnitPlay>
    {//伏击：2回合后，在回合开始时翻开。
        public VriheddSappers(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && Card.IsAliveOnPlance() && Card.Status.IsCountdown && Card.Status.Conceal)
            {
                await Card.Effect.SetCountdown(offset: -1);
                if (Card.Effect.Countdown <= 0)
                {
                    await Card.Effect.Ambush();
                }
            }
        }

        public async Task HandleEvent(BeforeUnitPlay @event)
        {
            if (@event.PlayedCard != Card)
            {
                return;
            }
            await Card.Effect.PlanceConceal(Card);
            await Card.Effect.SetCountdown(value: 2);
            return;
        }
    }
}