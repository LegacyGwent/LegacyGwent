using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70172")]//图尔赛克家族驯兽师
    public class Princess : CardEffect, IHandlesEvent<AfterTurnStart>
    {// Deploy: Spawn and Play a Bear. At the start of each of your turns, transform a Bear on the same row into a Raging Bear.
        public Princess(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            // Deploy: Spawn and Play a Giant Bear.
            await Game.CreateCard("15010", Card.PlayerIndex, new CardLocation(RowPosition.MyStay, 0));
            return 1;
        }
        public async Task HandleEvent(AfterTurnStart @event)
        {
            // At the start of each of your turns, transform a Giant Bear on the same row into a Raging Bear.
            var target = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow).Where(x => x.Status.CardId == "15010").FirstOrDefault();
            if (target != null && Card.Status.CardRow.IsOnPlace() && @event.PlayerIndex == Card.PlayerIndex)
            {
                await target.Effect.Transform(CardId.RagingBear, Card, isForce: true);
            }
            return;
        }
    }
}
