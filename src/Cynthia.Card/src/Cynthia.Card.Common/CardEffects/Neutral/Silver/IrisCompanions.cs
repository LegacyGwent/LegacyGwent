using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("13004")]//爱丽丝的同伴
    public class IrisCompanions : CardEffect
    {//Draw a card, then discard a random card. If you Iris: Shade is on the board, choose the card to discard instead.
        public IrisCompanions(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //己方卡组乱序呈现
            var list = Game.PlayersDeck[PlayerIndex].Mess(RNG).ToList();
            //让玩家选择一张卡,不能不选
            var result = await Game.GetSelectMenuCards(PlayerIndex, list, isCanOver: true);
            if (result.Count == 0) return 0;//如果没有任何符合标准的牌,返回
            var dcard = result.Single();
            var row = Game.RowToList(dcard.PlayerIndex, dcard.Status.CardRow);
            await Game.LogicCardMove(dcard, row, 0);//将选中的卡移动到最上方
            await Game.PlayerDrawCard(PlayerIndex);//抽卡
                                                   //---------------------------------------------------------------------------
                                                   //随机弃掉一张
            var IrisCount = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow).IgnoreConcealAndDead().Where(x => x.Status.CardRow.IsOnPlace() && x.Status.CardId == "70154" && x.Status.IsLock == false).ToList().Count();
            if (IrisCount > 0)
                //如果有爱丽丝,则让玩家选择要弃掉的牌
            {
                var discardcard = await Game.GetSelectMenuCards(PlayerIndex, Game.PlayersHandCard[PlayerIndex], isCanOver: true);
                await discardcard.Single().Effect.Discard(Card);
            }
            else
            {
                var discardcard = Game.PlayersHandCard[PlayerIndex].Mess(RNG).First();
                await discardcard.Effect.Discard(Card);
            }
            return 0;
        }
    }
}
