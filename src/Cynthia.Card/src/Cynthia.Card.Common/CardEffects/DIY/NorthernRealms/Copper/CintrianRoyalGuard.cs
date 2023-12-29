using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70126")]//辛特拉皇家护卫 CintrianRoyalGuard
    public class CintrianRoyalGuard : CardEffect
    {//己方总点数落后时，使同名牌获得3点增益。
        public CintrianRoyalGuard(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if ((Game.GetPlayersPoint(PlayerIndex) > Game.GetPlayersPoint(AnotherPlayer)))
            {
                return 0;
            }
            
            var cards = Game.GetPlaceCards(PlayerIndex).FilterCards(filter: x => x.Status.CardId == Card.Status.CardId).ToList();
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var card in cards)
            {
                await card.Effect.Boost(3, Card);
            }
            return 0;
        }
    }
}
