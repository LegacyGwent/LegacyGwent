using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130070")]//斯崔葛布：晋升
    public class StregoborPro : CardEffect
    {//休战：双方各抽1张单位牌，将其战力设为1。
        public StregoborPro(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            if (Game.IsPlayersPass[Game.AnotherPlayer(Card.PlayerIndex)]) return 0;

            var list2 = Game.PlayersDeck[Game.AnotherPlayer(Card.PlayerIndex)].Where(x => x.Status.Type == CardType.Unit).Take(2).ToList();
            //让玩家(另一半场)选择一张卡,不能不选
            var result2 = await Game.GetSelectMenuCards(Card.PlayerIndex, list2, isCanOver: false);
            if (result2.Count == 0) return 0;
            var dcard2 = result2.Single();
            var row2 = Game.RowToList(dcard2.PlayerIndex, dcard2.Status.CardRow);
            await Game.LogicCardMove(dcard2, row2, 0);//将选中的卡移动到最上方

            var cards = await Game.DrawCard(1, 1, x => x.CardInfo().CardType == CardType.Unit);
            foreach (var item in cards.Item1.Concat(cards.Item2))
            {
                await item.Effect.Damage(item.ToHealth().health - 1, item, BulletType.Lightnint);
            }
            return 0;
        }
    }
}