using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("32001")]//亚特里的林法恩
    public class RainfarnOfAttre : CardEffect
    {
        public RainfarnOfAttre(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            //打乱己方卡组取出所有间谍或双面间谍
            var list = Game.PlayersDeck[Card.PlayerIndex]
            .Where(x => (x.CardInfo().CardUseInfo == CardUseInfo.AnyRow || x.CardInfo().CardUseInfo == CardUseInfo.EnemyRow)).Mess();
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