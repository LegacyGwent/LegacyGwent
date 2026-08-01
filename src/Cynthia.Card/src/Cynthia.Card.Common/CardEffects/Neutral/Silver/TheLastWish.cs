using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13037")]//最后的愿望
    public class TheLastWish : CardEffect
    {//随机检视牌组的2张牌，打出1张。
        public TheLastWish(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            //打乱己方卡组,并且取2张卡
            var list = Game.PlayersDeck[PlayerIndex]
                .Mess(RNG).Take(2);
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
            (PlayerIndex, list.ToList(), 1, "选择打出一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0) return 0;
            await result.Single().MoveToCardStayFirst();
            return 1;
        }
    }
}