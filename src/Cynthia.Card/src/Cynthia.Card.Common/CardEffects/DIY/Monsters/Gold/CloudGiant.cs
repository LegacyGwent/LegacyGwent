using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70170")]// CloudGiant
    public class CloudGiant : CardEffect, IHandlesEvent<AfterTurnStart>
    {// On turn start, if you control the highest-powered unit, gain Immunity. If you don't, loose Immunity
        public CloudGiant(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        // is ally highest
        // gain immune on deploy if you control the highest-powered unit
        {
            // check if ally is highest
            var cards = Game.GetAllCard(Game.AnotherPlayer(Card.PlayerIndex)).Where(x=>x.Status.CardRow.IsOnPlace()).WhereAllHighest().ToList();
            bool allyishighest = false;
            foreach (var boardcard in cards)
            {
            if (boardcard.PlayerIndex == Card.PlayerIndex)
                {
                    allyishighest = true;
                }
            }
            if (allyishighest)
                {
                    Card.Status.IsImmue = true;
                }
            // gain resilience
            await Card.Effect.Resilience(Card);
            return 0;            
        }
        // On turn start, if you control the highest-powered unit, gain Immunity. If you don't, loose Immunity
        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            // check if ally is highest
            var cards = Game.GetAllCard(Game.AnotherPlayer(Card.PlayerIndex)).Where(x=>x.Status.CardRow.IsOnPlace()).WhereAllHighest().ToList();
            bool allyishighest = false;
            foreach (var boardcard in cards)
            {
            if (boardcard.PlayerIndex == Card.PlayerIndex)
                {
                    allyishighest = true;
                }
            }
            // loose or gain immunity
            if (allyishighest)
            {
                // weaken/strengthen is there to update the immune tag
                await Card.Effect.Strengthen(1,Card);
                await Card.Effect.Weaken(1,Card);
                Card.Status.IsImmue = true;
                return;
            }
            // weaken/strengthen is there to update the immune tag
            await Card.Effect.Strengthen(1,Card);
            await Card.Effect.Weaken(1,Card);
            Card.Status.IsImmue = false;
            return;
        }
    }
}
