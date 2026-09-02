using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70083")]//红骑士
    public class RedRider : CardEffect, IHandlesEvent<BeforeCardToCemetery>
    {
        public RedRider(GameCard card) : base(card) { }

        public async Task HandleEvent(BeforeCardToCemetery @event)
        {
            if (@event.isRoundEnd ||
                @event.Target.Status.Type != CardType.Unit ||
                @event.Target.PlayerIndex != AnotherPlayer ||
                !Card.Status.CardRow.IsInDeck() ||
                !@event.DeathLocation.RowPosition.IsOnPlace())
            {
                return;
            }

            var rowEffect = Game.GameRowEffect[@event.Target.PlayerIndex]
                [@event.DeathLocation.RowPosition.MyRowToIndex()];
            if (rowEffect.RowStatus != RowStatus.BitingFrost)
            {
                return;
            }

            await SetCountdown(offset: -1);
            if (Countdown > 0)
            {
                return;
            }

            await SetCountdown(value: 3);
            var location = Game.GetRandomCanPlayLocation(Card.PlayerIndex, true);
            if (location != null)
            {
                await Card.Effect.Summon(location, Card);
            }
        }
    }
}
