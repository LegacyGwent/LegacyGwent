using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23018")]//无骨者
    public class Maerolorn : CardEffect
    {//从牌组打出1张拥有遗愿能力的铜色单位牌。
        public Maerolorn(IGwentServerGame game, GameCard card) : base(game, card){}
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            //打乱己方卡组并取出拥有遗愿能力的铜色单位牌
            var list = Game.PlayersCemetery[PlayerIndex]
                .Where(x => (x.CardInfo().Info.Contains("遗愿："))//<这个位置等待category更新后改进>
                    && (x.Status.Group == Group.Silver || x.Status.Group == Group.Silver)).Mess();

            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
                (Card.PlayerIndex, list.ToList(), 1, "选择打出一张牌");

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