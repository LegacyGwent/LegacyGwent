using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("14001")]//侦查
    public class Reconnaissance : CardEffect
    {
        public Reconnaissance(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardUseEffect()
        {
            //打乱己方卡组,并且取2张铜色卡
            var list = Game.PlayersDeck[Card.PlayerIndex]
            .Where(x => x.Status.Group == Group.Copper && x.CardInfo().CardType == CardType.Unit).Mess().Take(2);
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