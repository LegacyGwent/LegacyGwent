using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53019")]//自然的馈赠
    public class NatureSGift : CardEffect
    {//从牌组打出1张铜色/银色“特殊”牌。
        public NatureSGift(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            //打乱己方卡组,并且选择所有铜色银色特殊牌
            var list = Game.PlayersDeck[Card.PlayerIndex]
            .Where(x => (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) && x.CardInfo().CardType == CardType.Special).Mess(RNG);
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
            (Card.PlayerIndex, list.ToList(), 1, "选择打出一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0) return 0;
            await result.First().MoveToCardStayFirst();
            return 1;
        }
    }
}