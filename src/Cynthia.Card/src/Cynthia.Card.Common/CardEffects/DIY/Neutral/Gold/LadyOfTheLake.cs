using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
namespace Cynthia.Card
{
    [CardEffectId("70006")]//湖中仙女
    public class LadyOfTheLake : CardEffect
    {//对自身造成削弱，削弱数值等同于手牌和牌库剩余卡牌之和的两倍。
        public LadyOfTheLake(GameCard card) : base(card) { }
        private const int mutipleFactor = 2;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            int weakenPointFromHand = Game.PlayersHandCard[Card.PlayerIndex].Count();
            int weakenPointFromDeck = Game.PlayersDeck[Card.PlayerIndex].Count();
            int totalWeakenPoint = (weakenPointFromHand + weakenPointFromDeck) * mutipleFactor;

            await Card.Effect.Weaken(totalWeakenPoint, Card);

            return 0;
        }
    }
}