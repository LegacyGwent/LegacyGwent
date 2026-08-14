using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId(CardId.GirlWhoDrankBrokilonWater)]
    public class GirlWhoDrankBrokilonWater : CardEffect, IHandlesEvent<AfterTurnStart>
    {
        public GirlWhoDrankBrokilonWater(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            for (var rowIndex = 0; rowIndex < 3; rowIndex++)
            {
                var row = rowIndex.IndexToMyRow();
                if (row == Card.Status.CardRow)
                {
                    continue;
                }

                await Game.CreateCardAtEnd(CardId.GirlWhoDrankBrokilonWater, PlayerIndex, row,
                    copy => copy.IsDoomed = true);
            }

            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            var strongestDryad = Game.RowToList(PlayerIndex, Card.Status.CardRow)
                .Where(candidate => candidate != Card
                    && candidate.IsAliveOnPlance()
                    && !candidate.Status.Conceal
                    && candidate.CardInfo().CardType == CardType.Unit
                    && candidate.HasAnyCategorie(Categorie.Dryad))
                .OrderByDescending(candidate => candidate.CardPoint())
                .FirstOrDefault();

            if (strongestDryad == null)
            {
                return;
            }

            await Card.Effect.Transform(strongestDryad.Status.CardId, Card);
        }
    }
}
