using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70110")]//哈吉的伊斯贝尔 Isbel of Hagge
    public class IsbelofHagge : CardEffect
    {//生成并打出对方牌组底端卡牌的1张原始同名牌。

        public IsbelofHagge(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            //await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, cards.Count), selectCard.Single());
            if (Game.PlayersDeck[AnotherPlayer].Count == 0)
                return 0;
            var lastCard = Game.PlayersDeck[AnotherPlayer].Last();
            await Game.CreateCard(lastCard.CardInfo().CardId,PlayerIndex,new CardLocation(RowPosition.MyStay,0));
            return 1;
        }
    }
}