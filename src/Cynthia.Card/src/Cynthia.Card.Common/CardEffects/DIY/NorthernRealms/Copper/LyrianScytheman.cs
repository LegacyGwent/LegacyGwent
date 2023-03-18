using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;


namespace Cynthia.Card
{
    [CardEffectId("70118")]//莱里亚镰刀手 LyrianScytheman
    public class LyrianScytheman : CardEffect
    {//随机使牌库一个单位获得2点增益，然后自身获得等同于该单位增益数的增益。

        public LyrianScytheman(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.CardInfo().CardType == CardType.Unit)
                .Mess(Game.RNG)
                .ToList().Take(1);

            if (cards.Count() == 0)
            {
                return 0;
            }
        
            foreach (var card in cards)
            {
                await card.Effect.Boost(3, Card);
            }
            var targets = cards.Single();
            var point = targets.CardPoint() - targets.Status.Strength;
            await Card.Effect.Boost(point, Card);
            return 0;
        }
        
       
    }
}
