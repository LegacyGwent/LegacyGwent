using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70100")]//林语者 ForestWhisperer
    public class ForestWhisperer : CardEffect, IHandlesEvent<AfterAmbushDown>
    {
        public ForestWhisperer(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterAmbushDown @event)
        {
            if (!Card.Status.CardRow.IsInDeck() ||
                @event.Target.PlayerIndex != PlayerIndex ||
                !@event.Target.Status.Conceal ||
                !@event.Target.HasAnyCategorie(Categorie.Ambush))
            {
                return;
            }

            var concealedAmbushes = Game.GetPlaceCards(PlayerIndex, isHasConceal: true)
                .Count(x => x.Status.Conceal && x.HasAnyCategorie(Categorie.Ambush));
            if (concealedAmbushes >= 2)
            {
                await Card.Effect.Summon(Game.GetRandomCanPlayLocation(PlayerIndex, true), @event.Target);
            }
        }
    }
}
