using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70135")]//斯瓦勃洛牧师 SvalblodPriest
    public class SvalblodPriest : CardEffect, IHandlesEvent<AfterCardHurt>
    {
        public SvalblodPriest(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await SetCountdown(2);
            return 0;
        }

        public async Task HandleEvent(AfterCardHurt @event)
        {
            if (!Card.Status.CardRow.IsOnPlace() ||
                @event.Target.PlayerIndex != PlayerIndex ||
                !Card.GetRangeCard(1, GetRangeType.HollowAll, isHasDead: true)
                    .Contains(@event.Target))
            {
                return;
            }

            await SetCountdown(offset: -1);
            if (Countdown > 0)
            {
                return;
            }

            await SetCountdown(2);
            await Card.Effect.Strengthen(1, Card);
        }
    }
}
