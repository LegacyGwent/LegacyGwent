using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70146")]//加尔 Gael
    public class Gael : CardEffect, IHandlesEvent<AfterTurnStart>
    {
        public Gael(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await ApplyGoldenFrothDrain();
            await Card.Effect.SetCountdown(3);
            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex ||
                !Card.Status.CardRow.IsOnPlace() ||
                Card.Status.Countdown <= 0)
            {
                return;
            }

            await Card.Effect.SetCountdown(offset: -1);
            if (Card.Status.Countdown == 0)
            {
                await ApplyGoldenFrothDrain();
            }
        }

        private async Task ApplyGoldenFrothDrain()
        {
            var rowIndex = Card.Status.CardRow.MyRowToIndex();
            await Game.GameRowEffect[AnotherPlayer][rowIndex]
                .SetStatus<GoldenFrothStatus>();

            var targets = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror())
                .IgnoreConcealAndDead()
                .ToList();
            foreach (var target in targets)
            {
                await Card.Effect.Drain(2, target);
            }
        }
    }
}
