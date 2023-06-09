using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70088")]//on turn end, damage all damaged enemies on the opposite row by 1
    public class TuirseachInvader : CardEffect, IHandlesEvent<AfterTurnOver>
    {
        public TuirseachInvader(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {//2 armor
            await Card.Effect.Armor(2, Card);
            return 0;
        }
        public async Task HandleEvent(AfterTurnOver @event)
        {//on turn end
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            var cards = Game.GetPlaceCards(AnotherPlayer).ToList().Where(x => (x.Status.CardRow == Card.Status.CardRow));
            foreach (var card in cards)
            {//damage all damaged enemies on the opposite row by 1
                if (card.Status.HealthStatus < 0)
                {
                    await card.Effect.Damage(1, Card);
                }
            }
            return;
        }
    }
}  