using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24002")]//鹿首魔
    public class Fiend : CardEffect
    {
        public Fiend(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selected = await Game.GetSelectPlaceCards(
                Card,
                filter: x => x.Status.CardRow != Card.Status.CardRow &&
                             x.HasAnyCategorie(Categorie.Beast),
                selectMode: SelectModeType.MyRow);
            if (selected.TrySingle(out var target))
            {
                await target.Effect.Move(
                    new CardLocation(Card.Status.CardRow, int.MaxValue),
                    Card);
            }
            return 0;
        }
    }
}
