using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13015")]//欧吉尔德·伊佛瑞克
    public class OlgierdVonEverec : CardEffect, IHandlesEvent<AfterCardToCemetery>, IHandlesEvent<BeforeRoundStart>//, IHandlesEvent<AfterRoundOver>, IHandlesEvent<AfterTurnStart>
    {//进入墓场时，复活自身，但战力削弱一半。When it enters the graveyard, it revives itself, but its battle power is weakened by half.  
        public OlgierdVonEverec(GameCard card) : base(card) { }

        private CardLocation _toRecurretLocation = null;
        public async Task HandleEvent(AfterCardToCemetery @event)
        {
            // 进入墓地的不是本卡，什么都不发生 Nothing happens if the card that enters the graveyard is not this card  
            if (@event.Target != Card)
            {
                return;
            }

            if (@event.isRoundEnd)
            {
                _toRecurretLocation = @event.DeathLocation;
                return;
            }
            await Task.CompletedTask;
        }
        public async Task HandleEvent(BeforeRoundStart @event)
        {
            //削弱值向上取整     //Weakness value is rounded up /
            var WeakenValue = (Card.Status.Strength + 1) / 2;
            if (Card.Status.CardRow.IsInCemetery())
            //放逐，不复活    //Exile, no resurrection 
            {
                if (Card.Status.Strength == WeakenValue)
                {
                    await Card.Effect.Weaken(WeakenValue, Card);
                    return;
                }
                if (_toRecurretLocation != null)
                {
                await Card.Effect.Resurrect(_toRecurretLocation, Card);
                }
                else
                {
                await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), Card);
                }
                await Card.Effect.Weaken(WeakenValue, Card);
            }
        }
    }
}
