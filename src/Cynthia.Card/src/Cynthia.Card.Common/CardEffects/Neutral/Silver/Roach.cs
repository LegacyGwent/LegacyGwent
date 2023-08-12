using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("13001")]//萝卜
    public class Roach : CardEffect, IHandlesEvent<AfterUnitDown>, IHandlesEvent<AfterUnitPlay>
    {
        public Roach(GameCard card) : base(card) { }

        private bool canSummon = false;

        public async Task HandleEvent(AfterUnitPlay @event)
        {
            if (@event.PlayedCard.Status.Group == Group.Gold && @event.PlayedCard.PlayerIndex == Card.PlayerIndex
                && Card.Status.CardRow.IsInDeck() && !@event.IsSpying)
            {
                canSummon = true;
                return;
            }
            await Task.CompletedTask;
            return;
        }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.Target.Status.Group == Group.Gold && @event.Target.PlayerIndex == Card.PlayerIndex
                && Card.Status.CardRow.IsInDeck() && !@event.IsSpying && !@event.IsMoveInfo.isMove && canSummon)
            {
                await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), @event.Target);
            }
        }
    }
}