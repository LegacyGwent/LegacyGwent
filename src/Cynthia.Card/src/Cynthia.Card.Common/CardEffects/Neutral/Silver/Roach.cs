using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("13001")]//萝卜
    public class Roach : CardEffect, IHandlesEvent<AfterUnitDown>
    {
        public Roach(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.Target.Status.Group == Group.Gold && @event.Target.PlayerIndex == Card.PlayerIndex
                && Card.Status.CardRow.IsInDeck() && !@event.IsSpying && !@event.IsMoveInfo.isMove)
            {
                await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), @event.Target);
            }
        }
    }
}
