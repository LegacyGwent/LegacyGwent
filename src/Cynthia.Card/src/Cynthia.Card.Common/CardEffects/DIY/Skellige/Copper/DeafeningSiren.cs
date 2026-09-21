using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
namespace Cynthia.Card
{
    [CardEffectId("70116")]//尖啸女海妖 DeafeningSiren
    public class DeafeningSiren : CardEffect, IHandlesEvent<AfterWeatherApply>
    {//己方回合中，每当敌方半场降下“倾盆大雨”时，从墓场将此单位召唤至随机一排。 每当敌方半场没有“倾盆大雨”时，将此单位移至墓场。
        // During your turn, whenever “Torrential Rain” is applied to the opponent’s side, summon this unit from the graveyard to a random row. whenever there is no “Torrential Rain” on the opponent's side of the board, move this unit to the graveyard."
        public DeafeningSiren(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterWeatherApply @event)
        {
            if (@event.Type == RowStatus.TorrentialRain && @event.PlayerIndex == AnotherPlayer && Card.Status.CardRow.IsInCemetery())
            {
                await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, false), Card);
            }
            bool hasTorrentialRain = Game.GameRowEffect[AnotherPlayer].Any(x => x.RowStatus == RowStatus.TorrentialRain);
            if (@event.Type != RowStatus.TorrentialRain && Card.Status.CardRow.IsOnPlace() && !hasTorrentialRain)
            {
                await Card.Effect.ToCemetery();
            }
            
            return;
        }
    }
}