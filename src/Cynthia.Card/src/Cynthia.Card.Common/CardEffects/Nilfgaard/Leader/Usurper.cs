using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("31004")]//篡位者
    public class Usurper : CardEffect
    {//间谍。不限阵营地创造1张领袖牌，使其获得2点增益。
        public Usurper(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var createCards = GwentMap.GetCreateCardsId(x => x.Group == Group.Leader && x.CardId != CardId.Usurper, RNG);
            var count = await Game.CreateAndMoveStay(
                Game.AnotherPlayer(Card.PlayerIndex),
                createCards.ToArray(),
                1);
            if (count == 0) return 0;
            await Game.RowToList(AnotherPlayer, RowPosition.MyStay).First().Effect.Boost(2, Card);
            return 1;
        }
    }
}