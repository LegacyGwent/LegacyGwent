using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70058")]//洞穴巨魔 CaveTroll
    public class CaveTroll2 : CardEffect, IHandlesEvent<AfterTurnStart>
    {//回合开始时，使1个战力最高的敌军单位获得3点增益，然后自身获得3点增益。
        public CaveTroll2(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() 
                && x.PlayerIndex != Card.PlayerIndex).WhereAllHighest().Mess(RNG).ToList();
                if (cards.Count() == 0)
                {
                    return ;
                }
                await cards.Mess(RNG).First().Effect.Boost(2, Card);
                await Card.Effect.Boost(2, Card);
            }
        }
    }
}
