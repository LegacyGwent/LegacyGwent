using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("12001")]//皇家谕令
    public class RoyalDecree : CardEffect
    {
        public RoyalDecree(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            //选择卡组中的金色单位
            var list = Game.PlayersDeck[Card.PlayerIndex]
            .Where(x => x.Status.Group == Group.Gold && x.CardInfo().CardType == CardType.Unit).Mess(RNG);
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
            (Card.PlayerIndex, list.ToList());
            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0) return 0;
            await result.First().MoveToCardStayFirst();
            await result.First().Effect.Boost(2);
            return 1;
        }
    }
}