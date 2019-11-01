using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13015")]//欧吉尔德·伊佛瑞克
    public class OlgierdVonEverec : CardEffect, IHandlesEvent<AfterCardDeath>, IHandlesEvent<AfterTurnOver>,IHandlesEvent<AfterRoundOver>
    {//遗愿：复活至原位。
        public OlgierdVonEverec(GameCard card) : base(card) { }
        private bool _resurrectFlag = false;
        private CardLocation _resurrectTarget = null;

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && _resurrectFlag == true)
            {
                await Card.Effect.Resurrect(_resurrectTarget, Card);
                _resurrectFlag = false;
            }
        }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card) return;
            _resurrectTarget = @event.DeathLocation;
            _resurrectFlag = true;
            await Task.CompletedTask;
            return;
        }

        public async Task HandleEvent(AfterRoundOver @event)
        {
            _resurrectFlag = false;
            await Task.CompletedTask;
        }
    }
}