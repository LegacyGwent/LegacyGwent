using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("34002")]//特使
    public class Emissary : CardEffect
    {
        public Emissary(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            //打乱己方卡组,并且取2张铜色卡
            var list = Game.PlayersDeck[Game.AnotherPlayer(Card.PlayerIndex)]
            .Where(x => x.Status.Group == Group.Copper && x.CardInfo().CardType == CardType.Unit).Mess(Game.RNG).Take(2);
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
            (Game.AnotherPlayer(Card.PlayerIndex), list.ToList(), 1, "选择打出一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0) return 0;
            await result.Single().MoveToCardStayFirst();
            return 1;
        }
    }
}