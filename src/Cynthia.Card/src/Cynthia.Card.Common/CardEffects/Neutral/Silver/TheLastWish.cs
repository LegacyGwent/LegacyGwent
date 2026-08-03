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
            var inspectCount = 2 + Game.PlayersCemetery[PlayerIndex]
                .Count(x => x.Status.CardId == CardId.MagicLamp);
            //打乱己方牌组，并检视2张牌；墓场中每有1张神灯便额外检视1张。
            var list = Game.PlayersDeck[PlayerIndex]
                .Mess(RNG).Take(inspectCount);
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
