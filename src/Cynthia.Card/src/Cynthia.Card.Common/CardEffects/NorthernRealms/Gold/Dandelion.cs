using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("42001")]//丹德里恩
    public class Dandelion : CardEffect
    {//使牌组3个单位获得2点增益。
        public Dandelion(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //乱序列出所有单位
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.CardInfo().CardType == CardType.Unit)
                .Mess(Game.RNG)
                .ToList();

            if (list.Count() == 0)
            {
                return 0;
            }
            //选一张，如果没选，什么都不做
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 3, "选择三张卡牌");
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