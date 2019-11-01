using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44029")]//泰莫利亚步兵
    public class TemerianInfantryman : CardEffect
    {//召唤所有同名牌。
        public TemerianInfantryman(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
            foreach (var card in cards)
            {
                await card.Effect.Summon(Card.GetLocation() + 1, Card);
            }
            return 0;
        }
    }
}