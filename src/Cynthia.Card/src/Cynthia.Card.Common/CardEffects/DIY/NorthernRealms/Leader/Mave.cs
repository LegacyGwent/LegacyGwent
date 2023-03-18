using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70045")]//米薇
    public class Mave : CardEffect
    {//使己方半场、手牌、牌库各1个单位获得5点增益。
        public Mave(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //半场
            var cards = await Game.GetSelectPlaceCards(Card, 1, selectMode: SelectModeType.MyRow);
            if (cards.Count() > 0)
            {
                await cards.Single().Effect.Boost(5, Card);
            }
            
            //手牌
            var hand_cards = Game.PlayersHandCard[Card.PlayerIndex].Where(x => (x.Status.Type == CardType.Unit));
            var list = hand_cards.ToList();
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
                (Card.PlayerIndex, list.ToList(), 1, "选择增益1张牌");
            //如果玩家一张卡都没选择,没有效果
            if (result.Any()) 
            {
                var card = result.Single();
                await card.Effect.Boost(5, Card);
            }

            //牌库
            var deck_list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.CardInfo().CardType == CardType.Unit)//乱序列出所有单位
                .Mess(Game.RNG)
                .ToList();

            if (deck_list.Count() == 0)
            {
                return 0;
            }
            //选一张，如果没选，什么都不做
            var deck_cards = await Game.GetSelectMenuCards(Card.PlayerIndex, deck_list, 1, "选择1张卡牌");
            if (deck_cards.Count() == 0)
            {
                return 0;
            }
            foreach (var card2 in deck_cards)
            {
                await card2.Effect.Boost(5, Card);
            }
            return 0;
        }
    }
}