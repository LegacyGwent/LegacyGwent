using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70180")]
    public class Albastra : CardEffect, IHandlesEvent<AfterTurnStart>
    {
        public Albastra(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            Card.Status.IsImmue = true;
            var wing1 = await Game.CreateCard(CardId.AlbastraLeftWing, PlayerIndex, Card.GetLocation());
            await wing1.Effect.Armor(3, Card);
            var wing2 = await Game.CreateCard(CardId.AlbastraRightWing, PlayerIndex, Card.GetLocation() + 1);
            await wing2.Effect.Armor(3, Card);
            await ApplyFrost();
            await Card.Effect.SetCountdown(2);
            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace()) return;

            int wingcount = Game.GetPlaceCards(PlayerIndex).Concat(Game.PlayersHandCard[PlayerIndex]).Concat(Game.PlayersDeck[PlayerIndex]).FilterCards(filter: x => x.Status.CardId == CardId.AlbastraLeftWing || x.Status.CardId == CardId.AlbastraRightWing).ToList().Count();
            if (wingcount <= 0)
            {
                await Card.Effect.ToCemetery(CardBreakEffectType.Scorch);
                return;
            }

            await Card.Effect.SetCountdown(offset: -1);
            if (Card.Effect.Countdown <= 0)
            {
                await Card.Effect.SetCountdown(2);
                await ApplyFrost();
            }
        }

        private async Task ApplyFrost()
        {
            if (!Card.Status.CardRow.IsOnPlace()) return;
            await Game.GameRowEffect[AnotherPlayer][Card.Status.CardRow.MyRowToIndex()]
                .SetStatus<BitingFrostStatus>();
        }
    }
}
