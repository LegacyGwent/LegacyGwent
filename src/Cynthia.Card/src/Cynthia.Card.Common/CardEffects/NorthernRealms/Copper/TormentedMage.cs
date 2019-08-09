using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44031")]//受折磨的法师
    public class TormentedMage : CardEffect
    {//检视牌组中2张铜色“法术”/“道具”牌，打出1张。
        public TormentedMage(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //打乱己方卡组,并且取2张铜色目标卡
            var list = Game.PlayersDeck[Card.PlayerIndex]
            .Where(x => x.Status.Group == Group.Copper && x.HasAnyCategorie(Categorie.Item, Categorie.Spell)).Mess(RNG).Take(2);
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