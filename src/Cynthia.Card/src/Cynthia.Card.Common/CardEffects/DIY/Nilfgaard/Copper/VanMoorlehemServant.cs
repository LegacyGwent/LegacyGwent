using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70127")]//莫拉汉姆家仆从 VanMoorlehemServant
    public class VanMoorlehemServant : CardEffect, IHandlesEvent<AfterCardConceal>
    {//
        public VanMoorlehemServant(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            Card.Status.IsImmue = true;
            await Task.CompletedTask;
            return 0;
        }

        public async Task HandleEvent(AfterCardConceal @event)
        {
            if (@event.Target != Card || @event.Source == null || @event.Source.PlayerIndex != Card.PlayerIndex) return;
            await Card.Effect.Boost(5, Card);
            return;
        }
    }
}
