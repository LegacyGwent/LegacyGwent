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
            var deck_list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.CardInfo().CardType == CardType.Unit && x.Status.Group == Group.Copper)//乱序列出所有单位
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
                await card.Effect.Boost(2, Card);
            }
            return 0;
        }
        
       
    }
}
