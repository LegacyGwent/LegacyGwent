using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70096")]//图尔赛克战船 TuirseachWarship
    public class TuirseachWarship : CardEffect, IHandlesEvent<AfterTurnOver>
    {//回合结束时，对1个未受伤的敌军随机单位造成1点伤害。
        public TuirseachWarship(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            var cards = Game.GetPlaceCards(AnotherPlayer).Concat(Game.GetPlaceCards(PlayerIndex)).FilterCards(filter: x => x.Status.HealthStatus >= 0).ToList();
            if (cards.Count() == 0)
            {
                return;
            }
            await cards.Mess(RNG).First().Effect.Damage(2, Card);
        }
    }
}
