using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62010")]//凯瑞丝：无所畏惧
    public class CerysFearless : CardEffect, IHandlesEvent<AfterCardDiscard>, IHandlesEvent<BeforePlayStayCard>
    {//复活己方下张丢弃的单位牌。
        public CerysFearless(GameCard card) : base(card) { }

        private GameCard _discardSource = null;
        private int _resurrectCount = 0;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.SetCountdown(value: 1);
            return 0;
        }
        public async Task HandleEvent(AfterCardDiscard @event)
        {
            if (Countdown <= 0)
            {
                return;
            }
            await SetCountdown(offset: -1);
            if (!Card.IsAliveOnPlance() ||
                @event.Target.PlayerIndex != PlayerIndex || @event.Source.PlayerIndex != PlayerIndex ||
                !@event.Target.Status.CardRow.IsInCemetery())
            {
                return;
            }

            await @event.Target.Effect.Resurrect(CardLocation.MyStayFirst, Card);
            _resurrectCount++;
            _discardSource = @event.Source;
        }

        public async Task HandleEvent(BeforePlayStayCard @event)
        {
            if (_discardSource == @event.Target && _discardSource != null)
            {
                @event.PlayCount += _resurrectCount;
                _discardSource = null;
                _resurrectCount = 0;
            }
            await Task.CompletedTask;
        }
    }
}