using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70072")]//雷蒂娅 Radeyah
    public class Radeyah : CardEffect
    {//造成等同于手牌中立牌数量的伤害。
        public Radeyah(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.PlayersHandCard[Card.PlayerIndex].Where(x => (x.Status.Faction == Faction.Neutral));
            for (var i = 0; i < 2; i++)
            {
                var result = await Game.GetSelectPlaceCards(Card);
                if (result.Count > 0)
                {
                    await result.Single().Effect.Damage(cards.Count(), Card);
                }
                
            }
            return 0;
        }
    }
}
