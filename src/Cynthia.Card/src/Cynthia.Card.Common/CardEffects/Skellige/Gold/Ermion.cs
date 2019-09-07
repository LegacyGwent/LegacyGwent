using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62004")]//莫斯萨克
    public class Ermion : CardEffect
    {//抽2张牌，随后丢弃2张牌。
        public Ermion(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (Game.PlayersDeck[Card.PlayerIndex].Count() == 1)
            {
                await Game.PlayerDrawCard(Card.PlayerIndex);
            }
            else if (Game.PlayersDeck[Card.PlayerIndex].Count() >= 2)
            {
                await Game.PlayerDrawCard(Card.PlayerIndex, 2);
            }
            //从手牌中选择2张弃牌,必须选
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, Game.PlayersHandCard[Card.PlayerIndex], 2, isCanOver: false);
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var target in cards)
            {
                await target.Effect.Discard(Card);
            }
            return 0;
        }
    }
}