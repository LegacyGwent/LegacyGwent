using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("52009")]//莫丽恩：森林之女
    public class MorennForestChild : CardEffect, IHandlesEvent<BeforeUnitPlay>, IHandlesEvent<BeforeSpecialPlay>
    {//伏击：当对方打出下张铜色/银色特殊牌时，翻开并抵消其能力。
        public MorennForestChild(GameCard card) : base(card) { }

        public async Task HandleEvent(BeforeUnitPlay @event)
        {
            if (@event.PlayedCard != Card)
            {
                return;
            }
            await Card.Effect.PlanceConceal(Card);
            return;
        }

        public async Task HandleEvent(BeforeSpecialPlay @event)
        {
            if (Card.IsAliveOnPlance() && Card.Status.Conceal && PlayerIndex != @event.Target.PlayerIndex)
            {
                await Ambush(async () =>
                {
                    await Task.CompletedTask;
                    @event.IsUse = false;
                });
            }
        }
    }
}