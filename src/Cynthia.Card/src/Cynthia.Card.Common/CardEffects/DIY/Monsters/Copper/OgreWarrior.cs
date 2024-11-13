using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70168")]// OgreWarrior
    public class OgreWarrior : CardEffect, IHandlesEvent<AfterTurnOver>
    {// On turn end, if you don't have the highest unit, damage self by half
        public OgreWarrior(GameCard card) : base(card) { }
        
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            var cards = Game.GetAllCard(Game.AnotherPlayer(Card.PlayerIndex)).Where(x=>x.Status.CardRow.IsOnPlace()).WhereAllHighest().ToList();
            bool allyishighest = false;
            foreach (var boardcard in cards)
                {
                if (boardcard.PlayerIndex == Card.PlayerIndex)
                    {
                        allyishighest = true;
                    }
                }
            if (!allyishighest)
                {
                    await Card.Effect.Damage((Card.Status.Strength + Card.Status.HealthStatus)/2, Card);
                }
            return;
        }
    }
}
