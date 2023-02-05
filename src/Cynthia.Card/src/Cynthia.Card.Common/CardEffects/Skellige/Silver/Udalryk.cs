using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63002")]//乌达瑞克

    public class Udalryk : CardEffect
    {//间谍、力竭。 检视牌组中2张牌。抽取1张，丢弃另1张。
        public Udalryk(GameCard card) : base(card) { }
        public bool IsUse = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (IsUse)
            {
                return 0;
            }

            IsUse = true;
            await Card.Effect.SetCountdown(offset: -1);
            //参考盖尔的写法,随机从我方卡组中选两张卡
            var card1 = Game.PlayersDeck[AnotherPlayer].Mess(Game.RNG).FirstOrDefault();
            var card2 = Game.PlayersDeck[AnotherPlayer].Mess(Game.RNG).FirstOrDefault(x => x != card1);

            var list = new List<GameCard>();

            if (card1 != default)
            {
                list.Add(card1);
            }
            if (card2 != default)
            {
                list.Add(card2);
            }
            //让玩家选择一张牌,必须选
            if (list.Count() == 0)
            {
                return 0;
            }
            //提示文字不确定 var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 1,"选择抽取一张牌");
            var result = await Game.GetSelectMenuCards(AnotherPlayer, list, 1, isCanOver: false);

            var dcard = result.Single();
            var row = Game.RowToList(dcard.PlayerIndex, dcard.Status.CardRow);
            await Game.LogicCardMove(dcard, row, 0);//将选中的卡移动到最上方
            await Game.PlayerDrawCard(dcard.PlayerIndex);//抽卡

            //如果有另一张卡
            if (list.Count() == 2)
            {
                foreach (var card in list)
                {
                    if (card != dcard)
                    {
                        await card.Effect.Discard(Card);
                    }
                }

            }

            return 0;



        }
    }
}