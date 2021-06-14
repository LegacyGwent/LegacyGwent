using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70059")]//独眼贝蒂 OneEyedBetsy
    public class OneEyedBetsy : CardEffect, IHandlesEvent<AfterTurnStart>
    {//回合开始时，自身获得3点增益，然后使1个战力最高的敌军单位获得3点增益。
        public OneEyedBetsy(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.Boost(3, Card);
                var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() 
                && x.PlayerIndex != Card.PlayerIndex).WhereAllHighest().Mess(RNG).ToList();
                if (cards.Count() == 0)
                {
                    return ;
                }
                await cards.Mess(RNG).First().Effect.Boost(3, Card);
            }
        }
    }
}
