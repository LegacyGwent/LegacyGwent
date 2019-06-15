using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54030")] //精灵佣兵
    public class ElvenMercenary : CardEffect
    {
        //随机检视牌组中2张铜色“特殊”牌，打出1张。
        public ElvenMercenary(IGwentServerGame game, GameCard card) : base(game, card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var cards = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x => x.Status.Group == Group.Copper && x.CardInfo().CardType == CardType.Special);
            var list = cards.Mess().Take(2);
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
                (Card.PlayerIndex, list.ToList(), 1, "选择打出一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0) return 0;
            await result.Single().MoveToCardStayFirst();
            return 1;
        }
    }
}