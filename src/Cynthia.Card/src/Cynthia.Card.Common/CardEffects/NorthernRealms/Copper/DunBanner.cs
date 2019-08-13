using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44023")]//褐旗营
    public class DunBanner : CardEffect, IHandlesEvent<AfterTurnStart>
    {//回合开始时，若落后25点战力以上，则召唤此单位至随机排。
        public DunBanner(GameCard card) : base(card) { }



        public async Task HandleEvent(AfterTurnStart @event)
        {
            var player1Row1Point = Game.PlayersPlace[Card.PlayerIndex][0].Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
            var player1Row2Point = Game.PlayersPlace[Card.PlayerIndex][1].Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
            var player1Row3Point = Game.PlayersPlace[Card.PlayerIndex][2].Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
            var player2Row1Point = Game.PlayersPlace[Game.AnotherPlayer(Card.PlayerIndex)][0].Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
            var player2Row2Point = Game.PlayersPlace[Game.AnotherPlayer(Card.PlayerIndex)][1].Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
            var player2Row3Point = Game.PlayersPlace[Game.AnotherPlayer(Card.PlayerIndex)][2].Select(x => x.Status).Sum(x => x.Strength + x.HealthStatus);
            var myPlacePoint = (player1Row1Point + player1Row2Point + player1Row3Point);
            var enemyPlacePoint = (player2Row1Point + player2Row2Point + player2Row3Point);
            if (myPlacePoint + 25 < enemyPlacePoint)
            {
                //召唤全部
                var cards = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
                foreach (var card in cards)
                {
                    await card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex), Card);
                }

            }

            return;

        }
    }
}