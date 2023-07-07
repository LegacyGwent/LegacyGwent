using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{

    [CardEffectId("70082")]//弑亲者恩约夫
    public class ArnjolfThePatricide : CardEffect
    {
        //择一：丢弃手牌中重复的牌，直至没有重复的牌，抽等量的牌；丢弃手牌中战斗力高于自身的牌，直至没有战力高于自身的牌，抽等量的牌。
        public ArnjolfThePatricide(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        // public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var switchCard = await Card.GetMenuSwitch(
                ("处罚", "丢弃手牌中重复的牌，直至没有重复的牌，抽等量的牌"),
                ("弑杀", "丢弃手牌中战斗力高于自身的牌，直至没有战力高于自身的牌，抽等量的牌")
            );
            if (switchCard == 0)
            {
                //丢弃手牌中重复的牌 
                var cards = Game.PlayersHandCard[PlayerIndex]
                    .GroupBy(x => x.Status.CardId)
                    .Where(y => y.Count() > 1)
                    .SelectMany(y => y.Take(y.Count() - 1))
                    .ToList();
                var count = cards.Count();
                foreach (var card in cards)
                {
                    await card.Effect.Discard(Card);
                }
                await Game.PlayerDrawCard(PlayerIndex, count);
                return 0;

            }                
            
            if (switchCard == 1)
            {
                //丢弃手牌中小于自身点数的牌，并抽牌
                var cards = Game.PlayersHandCard[PlayerIndex].Where(x => x.CardPoint() > Card.CardPoint()).ToList();
                var count = cards.Count();
                foreach (var card in cards)
                {
                    await card.Effect.Discard(Card);
                }
                await Game.PlayerDrawCard(PlayerIndex, count);
                return 0;
            }
            return 0;
        }
    }
}