using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13015")]//欧吉尔德·伊佛瑞克
    public class OlgierdVonEverec : CardEffect, IHandlesEvent<AfterCardToCemetery>, IHandlesEvent<BeforeRoundStart>
    {//小局开始时，复活自身并削弱一半战力。
        public OlgierdVonEverec(GameCard card) : base(card) { }
        private CardLocation _resurrectTarget;

        public Task HandleEvent(AfterCardToCemetery @event)
        {
            if (@event.Target == Card && @event.isRoundEnd)
            {
                _resurrectTarget = @event.DeathLocation;
            }

            return Task.CompletedTask;
        }

        public async Task HandleEvent(BeforeRoundStart @event)
        {
            if (!Card.Status.CardRow.IsInCemetery())
            {
                return;
            }

            var weakenValue = (Card.Status.Strength + 1) / 2;
            if (Card.Status.Strength == weakenValue)
            {
                await Card.Effect.Weaken(weakenValue, Card);
                return;
            }

            var location = _resurrectTarget ?? Game.GetRandomCanPlayLocation(Card.PlayerIndex, true);
            await Card.Effect.Resurrect(location, Card);
            await Card.Effect.Weaken(weakenValue, Card);
        }
    }
}
