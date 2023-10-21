using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23018")]//无骨者
    public class Maerolorn : CardEffect
    {//从牌组打出1张拥有遗愿能力的铜色单位牌。
        public Maerolorn(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //打乱己方卡组并取出拥有遗愿能力的铜色单位牌
            var list = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x => (x.Status.HideTags == null ? false : x.Status.HideTags.Contains(HideTag.Deathwish))
                    && (x.Status.Group == Group.Copper)
                    && (x.CardInfo().CardType == CardType.Unit))
                .Mess(Game.RNG)
                .ToList();

            if (list.Count() == 0)
            {
                return 0;
            }

            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1, "选择打出一张牌");

            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0)
            {
                return 0;
            }

            //打出
            await result.Single().MoveToCardStayFirst();
            return 1;
        }
    }
}
