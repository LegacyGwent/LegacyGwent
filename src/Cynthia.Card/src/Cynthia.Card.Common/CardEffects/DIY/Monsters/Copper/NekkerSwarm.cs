using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70169")]// NekkerSwarm
    public class NekkerSwarm : CardEffect, IHandlesEvent<AfterTurnOver>
    {// On turn end, if no enemy is bigger than your biggest ogroid on the board, decrease counter by 1. When it reaches 0 summon self on a random row and boost a random ally by 1
        public NekkerSwarm(GameCard card) : base(card) { }
        
        public async Task HandleEvent(AfterTurnOver @event)
        {
        // compare biggest ogroid ally with biggest enemy
        if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsInDeck())
            {
                return;
            }
        var list1 = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex == Card.PlayerIndex && x.HasAllCategorie(Categorie.Ogroid)).WhereAllHighest().ToList();
        var OgreMaximun = 0;
        if (list1.Count() != 0)
            {
                OgreMaximun = list1.First().CardPoint();
            }
        else {return;}
        var list2 = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex != Card.PlayerIndex).WhereAllHighest().ToList();
        var EnemyMaximun = 0;
        if (list2.Count() != 0)
            {
                EnemyMaximun = list2.First().CardPoint();
            }
        bool allyishighest = false;
        if (OgreMaximun >= EnemyMaximun)
            {
                allyishighest = true;
            }
        // summon and boost    
        if (Card.Status.CardRow.IsInDeck() && allyishighest)
            {
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
            if (list.Count() == 0)
            {
                return;
            }

            if (Card == list.Last())
            {
                // decrease counter
                await Card.Effect.SetCountdown(offset: -1);
                // summon and boost
                if (Card.Effect.Countdown <= 0)
                    {
                        await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), Card);
                        var allycards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex == Card.PlayerIndex).ToList();
                        if (allycards.Count() == 0)
                        {
                            return;
                        }
                        await allycards.Mess(Game.RNG).First().Effect.Boost(1, Card);
                    }
                }
            return;
            }
        return;
        }
    }
}
