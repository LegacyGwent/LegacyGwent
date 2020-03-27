using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70001")]//昆恩护盾
    public class Quen : CardEffect
    {//给予手牌中一个铜色单位及其牌组中的同名卡一层护盾，阻挡一次削弱/伤害/重置效果。
        public Quen(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //打乱己方卡组,并且取2张铜色卡
            var list = Game.PlayersDeck[Card.PlayerIndex]
            .Where(x => x.Status.Group == Group.Copper && x.CardInfo().CardType == CardType.Unit).Mess(RNG).Take(2);
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