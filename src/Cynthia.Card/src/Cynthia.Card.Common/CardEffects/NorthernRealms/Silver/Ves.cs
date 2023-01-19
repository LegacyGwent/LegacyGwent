using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("43002")]//薇丝
    public class Ves : CardEffect
    {//交换最多2张牌。
        public Ves(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //以下代码基于：卡组里有0张牌的时候，不能交换。其他情况下先把k张卡送入牌组 再抽k张牌 k<=2

            //如果牌库中没有牌 不能交换
            if (Game.PlayersDeck[PlayerIndex].Count() == 0)
            {
                return 0;
            }
            var selectList = Game.PlayersHandCard[PlayerIndex].ToList();
            var handresult = await Game.GetSelectMenuCards(PlayerIndex, selectList, 2, "选择交换张牌", isCanOver: true);
            int swapnum = handresult.Count();
            if (swapnum == 0)
            {
                return 0;
            }

            // pre-select swapnum cards from deck
            if(!Game.PlayersDeck[PlayerIndex].TryMessN(out var cardsFromDeck, Game.RNG, swapnum)) {
                return 0;
            }

            // swap cards based on the min cards between deck and swapnum
            for (int i = 0; i < Math.Min(swapnum, cardsFromDeck.Count()); i++)
            {
                await handresult[i].Effect.Swap(cardsFromDeck.ElementAt(i));
            }

            return 0;
            
            // foreach (var card in handresult)
            // {
            //     await card.Effect.Swap();
            // }
            // await Game.PlayerDrawCard(Card.PlayerIndex, swapnum);
            // return 0;
















            // //以下代码基于:卡组里只有一张卡的时候，只交换一张,有零张卡时不交换。
            // var deck = Game.PlayersDeck[PlayerIndex];
            // var selectList = Game.PlayersHandCard[PlayerIndex].ToList();
            // //最大可交换牌数是 2，手牌数，牌库剩余卡数 的最小值
            // var cardnumlist = new List<int>() { 2, deck.Count, selectList.Count };

            // if (cardnumlist.Min() == 0)
            // {
            //     return 0;
            // }
            // if (cardnumlist.Min() == 1)
            // {
            //     var handresult1 = await Game.GetSelectMenuCards(PlayerIndex, selectList, 1, "选择交换一张牌");
            //     if (handresult1.Count() == 0)
            //     {
            //         return 0;
            //     }
            //     await handresult1.First().Effect.Swap(deck.Mess(Game.RNG).First());
            //     return 0;

            // }
            // var handresult2 = await Game.GetSelectMenuCards(PlayerIndex, selectList, 2, "选择交换两张牌");
            // if (handresult2.Count() == 0)
            // {
            //     return 0;
            // }
            // var swapresult = deck.Mess(Game.RNG).Take(2).ToList();
            // for (var i = 0; i < 2; i++)
            // {
            //     await handresult2[i].Effect.Swap(swapresult[i]);
            // }
            // return 0;
        }
    }
}