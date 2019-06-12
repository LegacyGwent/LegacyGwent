using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("31004")]//篡位者
    public class Usurper : CardEffect
    {//间谍。不限阵营地创造1张领袖牌，使其获得2点增益。
        public Usurper(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var createCards = GwentMap.GetCreateCardsId(x => x.Group == Group.Leader && x.CardId != CardId.Usurper);
            return await Game.CreateAndMoveStay(
                Game.AnotherPlayer(Card.PlayerIndex),
                createCards.ToArray(),
                1,
                true);
        }
    }
}