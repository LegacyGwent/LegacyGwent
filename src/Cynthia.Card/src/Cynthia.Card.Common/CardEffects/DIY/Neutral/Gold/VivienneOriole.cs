using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70008")]//薇薇恩：月下金莺

    public class VivienneOriole : CardEffect, IHandlesEvent<AfterTurnOver>
    {//力竭，己方回合结束时，如果己方战力超过对方25点以上，则返回手牌。
        public VivienneOriole(GameCard card) : base(card) { }
        private bool _isUse = false;
        private const int thresholdPoint = 25;
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (_isUse == false && @event.PlayerIndex == PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                var pointDiff = Game.GetPlayersPoint(Card.PlayerIndex) - Game.GetPlayersPoint(Game.AnotherPlayer(Card.PlayerIndex));
                if (pointDiff > thresholdPoint)
                {
                    _isUse = true;
                    Card.Effect.Repair(true);
                    await Game.ShowCardMove(new CardLocation(RowPosition.MyHand, 0), Card);
                }
                return;
            }
        }
    }
}
