using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70126")]//辛特拉皇家护卫 CintrianRoyalGuard
    public class CintrianRoyalGuard : CardEffect
    {//
        public CintrianRoyalGuard(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var point = Game.GetPlayersPoint(Game.AnotherPlayer(Card.PlayerIndex)) - Game.GetPlayersPoint(Card.PlayerIndex);
            if (point <= 0) 
            {
                await Card.Effect.Boost(3,Card);
            }
            
            if (point > 0) 
            {
                var Ltaget = Card.GetRangeCard(1, GetRangeType.HollowLeft);
                if (Ltaget.Count() != 0 && !Ltaget.Single().Status.Conceal)
                {
                    await Ltaget.Single().Effect.Boost(3, Card); 
                }

                var Rtaget = Card.GetRangeCard(1, GetRangeType.HollowRight);
                if (Rtaget.Count() != 0 && !Rtaget.Single().Status.Conceal)
                {
                    await Rtaget.Single().Effect.Boost(3, Card);
                }
            }
            return 0;
        }
    }
}
