using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70116")]//尖啸女海妖 DeafeningSiren
    public class DeafeningSiren : CardEffect, IHandlesEvent<AfterWeatherApply>
    {//每当在对方半场降下“倾盆大雨”，复活自身至随机排。
        public DeafeningSiren(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterWeatherApply @event)
        {
            if (@event.Type == RowStatus.TorrentialRain && @event.PlayerIndex == AnotherPlayer)

            {
                if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsInCemetery())
                {
                    await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex, false), Card);
                }
                
                return;
            }
            return;
        }
    }
}