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
            var rows = Enumerable.Range(0, 3).Select(index => index.IndexToMyRow())
                .Where(row => row != Card.Status.CardRow && Game.RowToList(PlayerIndex, row).Count < Game.RowMaxCount)
                .ToList();
            if (rows.Count > 0)
            {
                var row = await Game.GetSelectRow(PlayerIndex, Card, rows);
                await Game.CreateCardAtEnd(CardId.GirlWhoDrankBrokilonWater, PlayerIndex, row,
                    copy => copy.IsDoomed = true, source: Card);
            }

            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (!Card.Status.CardRow.IsOnPlace())
            {
                return;
            }

            var strongestDryad = Game.RowToList(PlayerIndex, Card.Status.CardRow)
                .Where(candidate => candidate != Card
                    && candidate.CardInfo().Name != Card.CardInfo().Name
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
