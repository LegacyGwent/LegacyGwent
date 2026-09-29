using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId(CardId.VanMoorlehemServant)]//莫拉汉姆家仆从 VanMoorlehemServant
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
            if (@event.Target != Card || (int)Game.GameRound != Card.PlayerIndex) return;
            await Card.Effect.Boost(4, Card);
            await Card.Effect.Reveal(Card);
            return;
        }
    }
}
