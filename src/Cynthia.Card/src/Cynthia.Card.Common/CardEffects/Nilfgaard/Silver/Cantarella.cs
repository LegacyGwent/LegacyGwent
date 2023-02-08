using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("33004")]//坎塔蕾拉
    public class Cantarella : CardEffect
    {
        public Cantarella(GameCard card) : base(card) { }
        public bool IsUse = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //这张卡只能用一次
            if (IsUse) return 0;
            IsUse = true;
            await Card.Effect.SetCountdown(offset: -1);
            //选择卡组随机两张卡(另一半场玩家)
            var list = Game.PlayersDeck[Game.AnotherPlayer(Card.PlayerIndex)].Take(2).ToList();
            //让玩家(另一半场)选择一张卡,不能不选
            var result = await Game.GetSelectMenuCards(Game.AnotherPlayer(Card.PlayerIndex), list, isCanOver: false);
            if (result.Count == 0) return 0;
            var dcard = result.Single();
            var row = Game.RowToList(dcard.PlayerIndex, dcard.Status.CardRow);
            await Game.LogicCardMove(dcard, row, 0);//将选中的卡移动到最上方
            await Game.PlayerDrawCard(Game.AnotherPlayer(Card.PlayerIndex));//抽卡
            if (list.Count() > 1)//将另一张卡移动到末尾
                await Game.LogicCardMove(list.Where(x => x != dcard).Single(), row, row.Count);
            return 0;
        }
    }
}