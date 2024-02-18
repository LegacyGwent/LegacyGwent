using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70101")]//不朽者骑兵 ImmortalCavalry
    public class ImmortalCavalry : CardEffect, IHandlesEvent<AfterUnitDown>, IHandlesEvent<AfterTurnStart>, IHandlesEvent<OnGameStart>
    {//对局开始时锁定自身，己方打出下一张单位牌时，使其获得2点增益。
        public ImmortalCavalry(GameCard card) : base(card){}
        
        private bool isUse = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.SetCountdown(1);
            return 0;
        }
        public async Task HandleEvent(OnGameStart @event)
        {
            if (Card.Status.CardRow.IsInDeck() && Card.Status.IsLock == false)
            {
                await Card.Effect.Lock(Card);
            }
            return;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex == PlayerIndex && Card.IsAliveOnPlance() && isUse == false)
            {
                isUse = true;
            }
            await Task.CompletedTask;
        }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.Target.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace() ||Card.Effect.Countdown != 1)
            {
                return;
            }

            if (isUse == true)
            {
                await @event.Target.Effect.Boost(3, Card);
                await SetCountdown(offset: -1);
            }
            return;
        }
    }
}
