using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("70136")]//乌鸦
    public class Crow : CardEffect, IHandlesEvent<AfterCardDeath>
    {//
        public Crow(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card)
            {
                return;
            }
            var enemycards = Game.GetPlaceCards(AnotherPlayer).ToList();
            if (enemycards.Count() == 0)
            {
                return;
            }
            await enemycards.Mess(Game.RNG).First().Effect.Damage(3, Card);
            return;
        }
    }
}
