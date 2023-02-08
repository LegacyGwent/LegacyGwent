using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;


namespace Cynthia.Card
{
    [CardEffectId("43001")]//塔勒
    public class Thaler : CardEffect
    {//间谍、力竭。 抽2张牌，保留1张，放回另1张。
        public Thaler(GameCard card) : base(card) { }
        public bool IsUse { get; set; } = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (IsUse)
            {
                return 0;
            }
            IsUse = true;
            await Card.Effect.SetCountdown(offset: -1);
            //先选两张卡
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
            //对于抽取的卡
            var result = await Game.GetSelectMenuCards(AnotherPlayer, list, 1, "选择抽一张牌", isCanOver: false);
            var dcard = result.Single();
            var row = Game.RowToList(dcard.PlayerIndex, dcard.Status.CardRow);
            await Game.LogicCardMove(dcard, row, 0);//将选中的卡移动到最上方
            await Game.PlayerDrawCard(dcard.PlayerIndex);//抽卡

            //对于另一张卡，放回意味着这样卡到了另一个卡组随机位置
            if (list.Count() == 2)
            {
                foreach (var card in list)
                {
                    if (card != dcard)
                    {
                        //放回到随机位置
                        await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)), card);
                    }
                }

            }
            return 0;
        }
    }
}