using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("13001")]//萝卜
    public class Roach : CardEffect, IHandlesEvent<AfterUnitPlay>
    {
        public Roach(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterUnitPlay @event)
        {
            if (@event.PlayedCard.Status.Group == Group.Gold && @event.PlayedCard.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsInDeck())
            {
                await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), @event.PlayedCard);
            }
        }
    }
}