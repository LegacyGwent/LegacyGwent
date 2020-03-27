using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
namespace Cynthia.Card
{
    [CardEffectId("70006")]//湖中仙女
    public class LadyOfTheLake : CardEffect
    {//部署：对自身造成等同于己方牌组中剩余牌数量与手牌数量之和的削弱。
        public LadyOfTheLake(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            int weakenPointFromHand = Game.PlayersHandCard[Card.PlayerIndex].Count();
            int weakenPointFromDeck = Game.PlayersDeck[Card.PlayerIndex].Count();
            int totalWeakenPoint = weakenPointFromHand+ weakenPointFromDeck;

            await Card.Effect.Weaken(totalWeakenPoint, Card);

            return 0;
        }
    }
}