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
            await Card.Effect.SetCountdown(value: 2); //1 originally
            return 0;
        }
        public async Task HandleEvent(AfterCardDiscard @event)
        {
            // 现在关于触发源的触发条件：触发源在己方半场且不是密探，或者触发源在对方半场且是密探；然后这里要取反
            // 出新卡时注意可能涉及bug
            if (Countdown <= 0 || !Card.IsAliveOnPlance() || @event.Target.PlayerIndex != PlayerIndex 
            || (@event.Source.PlayerIndex == PlayerIndex && @event.Source.HasAnyCategorie(Categorie.Agent))
            || (@event.Source.PlayerIndex != PlayerIndex && !@event.Source.HasAnyCategorie(Categorie.Agent)))
            {
                return;
            }
            if (@event.Target.Status.Group != Group.Gold)
            {
                await Card.Effect.Damage(4, Card); // self damage added for balance
                await SetCountdown(offset: -1);
                if (!Card.IsAliveOnPlance() || !@event.Target.Status.CardRow.IsInCemetery())
                {
                    return;
                }

                await @event.Target.Effect.Resurrect(CardLocation.MyStayFirst, Card);
                _resurrectCount++;
                _discardSource = @event.Source;
            }
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