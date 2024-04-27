using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70117")]//法芙 Fauve
    public class Fauve : CardEffect, IHandlesEvent<AfterUnitDown>, IHandlesEvent<AfterTurnOver>
    {
        public Fauve(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.Target.Status.Group == Group.Leader && @event.Target.PlayerIndex == Card.PlayerIndex
                && Card.Status.CardRow.IsInDeck() && !@event.IsSpying && !@event.IsMoveInfo.isMove)
            {
                await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), @event.Target);
            }
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            var targets = Game.GetPlaceCards(PlayerIndex)
            .FilterCards(type: CardType.Unit, filter: x => x.CardPoint() == Card.CardPoint() && x != Card);

            foreach (var target in targets)
            {
                await target.Effect.Boost(1, Card);
            }
            return;
        }
    }
}
