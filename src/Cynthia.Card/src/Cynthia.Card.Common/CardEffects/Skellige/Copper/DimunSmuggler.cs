using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;
namespace Cynthia.Card
{
    [CardEffectId("64004")]//迪门家族走私贩
    public class DimunSmuggler : CardEffect
    {//将1个铜色单位从己方墓场返回至牌组。
        public DimunSmuggler(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = Game.PlayersCemetery[PlayerIndex].Where(x => x.Status.Group == Group.Copper && x.CardInfo().CardType == CardType.Unit);
            if (list.Count() == 0)
            {
                return 0;
            }
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
            (Card.PlayerIndex, list.ToList(), 1, "选择一张牌返回牌组");
            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0)
            {
                return 0;
            }
            //希里：冲刺的返回牌组机制，返回到随机位置
            var range = Game.RNG.Next(0, Game.PlayersDeck[PlayerIndex].Count() + 1);
            await result.Single().Effect.Resurrect(new CardLocation(RowPosition.MyDeck, range), result.Single());
            return 0;
        }
    }
}
