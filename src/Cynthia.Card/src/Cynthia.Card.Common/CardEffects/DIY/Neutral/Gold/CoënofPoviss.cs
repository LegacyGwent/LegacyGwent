using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70158")]
    public class CoënofPoviss : CardEffect, IHandlesEvent<AfterTurnOver>, IHandlesEvent<AfterCardDeath>
    {
        private const string FarmerCardId = "15011";

        public CoënofPoviss(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selected = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (selected.TrySingle(out var target))
            {
                await target.Effect.Damage(3, Card);
            }
            return 0;
        }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace()) return;

            for (var repeat = 0; repeat < 2; repeat++)
            {
                var targets = Game.GetPlaceCards(PlayerIndex)
                    .Where(card => card != Card && card.Status.Categories.Contains(Categorie.Witcher))
                    .WhereAllLowest()
                    .ToList();
                foreach (var target in targets)
                {
                    await target.Effect.Boost(1, Card);
                }
            }
        }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card || !@event.DeathLocation.RowPosition.IsOnPlace()) return;

            await Game.CreateCard(
                FarmerCardId,
                AnotherPlayer,
                new CardLocation(@event.DeathLocation.RowPosition, int.MaxValue));
        }
    }
}
