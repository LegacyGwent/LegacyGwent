using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;


namespace Cynthia.Card
{
    [CardEffectId("70094")]//莱里亚骑兵 LyrianCavalry
    public class LyrianCavalry : CardEffect
    {//xxxxxxx

        public LyrianCavalry(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var deck_list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.CardInfo().CardType == CardType.Unit)//乱序列出所有单位
                .Mess(Game.RNG)
                .ToList();
            if (deck_list.Count() == 0)
            {
                return 0;
            }
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, deck_list, 1, "选择1张卡牌");
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var card in cards)
            {
                var targets = cards.Single();
                var point = targets.CardPoint() - targets.Status.Strength;
                await Card.Effect.Boost(point, Card);
            }
            return 0;
        }
        
       
    }
}
