using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70117")]//法芙 Fauve
    public class Fauve : CardEffect, IHandlesEvent<AfterUnitDown>, IHandlesEvent<AfterCardDeath>
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

         public async Task HandleEvent(AfterCardDeath @event)
        {
            var deck = Game.PlayersDeck[PlayerIndex]
                .Where(card => card.Status.Categories.Contains(Categorie.Dryad))
                .ToList();

            if (deck.Count() == 0)
            {
                return;
            }

            await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, 0), deck.First());
        }
    }
}