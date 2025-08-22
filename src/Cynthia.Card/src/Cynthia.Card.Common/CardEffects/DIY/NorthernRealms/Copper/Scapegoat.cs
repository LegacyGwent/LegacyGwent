using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70184")]//替罪羊 Scapegoat
    public class Scapegoat : CardEffect, IHandlesEvent<AfterTurnOver>, IHandlesEvent<OnGameStart>
    {//
        public Scapegoat(GameCard card) : base(card){}
        
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

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex == PlayerIndex && Card.IsAliveOnPlance() && isUse == false)
            {
                isUse = true;
                await Game.CreateCard(Card.Status.CardId, PlayerIndex, Card.GetLocation());
                await Card.Effect.Lock(Card.GetRangeCard(1, GetRangeType.HollowRight).FirstOrDefault());
                await SetCountdown(offset: -1);
            }

            await Task.CompletedTask;
        }
    }
}
