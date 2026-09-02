using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70177")]//挠挠先生
    public class SirScratchALot : CardEffect, IHandlesEvent<AfterUnitDown>
    {
        public SirScratchALot(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            foreach (var half in new[] { PlayerIndex, AnotherPlayer })
            {
                var targetRows = Game.GameRowEffect[half].Indexed()
                    .Where(x => x.Value.RowStatus == RowStatus.FullMoon)
                    .Select(x => x.Key);
                foreach (var rowIndex in targetRows)
                {
                    foreach (var beast in Game.PlayersPlace[half][rowIndex]
                        .IgnoreConcealAndDead()
                        .FilterCards(filter: x => x.HasAllCategorie(Categorie.Beast))
                        .ToList())
                    {
                        await beast.Effect.Boost(1, Card);
                    }
                }
            }
            return 0;
        }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.Target.PlayerIndex != PlayerIndex || @event.IsSpying ||
                !@event.Target.HasAllCategorie(Categorie.Beast) ||
                !(Card.Status.CardRow.IsInHand() ||
                  Card.Status.CardRow.IsInDeck()))
            {
                return;
            }

            await Card.Effect.Boost(1, @event.Target);
        }
    }
}
