using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70116")]//尖啸女海妖 DeafeningSiren
    public class DeafeningSiren : CardEffect, IHandlesEvent<AfterWeatherApply>, IHandlesEvent<AfterTurnOver>
    {//On turn end, if there is rain on the opposite side of the board, resurect it
        public DeafeningSiren(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterWeatherApply @event)
        {
            if (@event.Type == RowStatus.TorrentialRain && @event.PlayerIndex == AnotherPlayer)
            {
                if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsInCemetery())
                {
                    await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex, false), Card);
                }
                
            }
            return;
        }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            var count = 0;
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsInCemetery())
            {return;}
            if (Game.GameRowEffect[AnotherPlayer][RowPosition.EnemyRow1.Mirror().MyRowToIndex()].RowStatus == RowStatus.TorrentialRain)
            {count += 1;}
            if (Game.GameRowEffect[AnotherPlayer][RowPosition.EnemyRow2.Mirror().MyRowToIndex()].RowStatus == RowStatus.TorrentialRain)
            {count += 1;}
            if (Game.GameRowEffect[AnotherPlayer][RowPosition.EnemyRow3.Mirror().MyRowToIndex()].RowStatus == RowStatus.TorrentialRain)
            {count += 1;}
            if (count == 0)
            {return;}
            await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex, false), Card);
            return;
        }
    }
}