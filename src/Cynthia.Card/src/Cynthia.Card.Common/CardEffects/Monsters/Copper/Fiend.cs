using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24002")]//if it's the only relict on the row damage self by 2
    public class Fiend : CardEffect
    {
        public Fiend(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = Game.RowToList(PlayerIndex, Card.Status.CardRow).IgnoreConcealAndDead().Where(x => x.HasAnyCategorie(Categorie.Relict) && x != Card);
            if (list.Count() == 0)
            {
                await Damage(2, Card);
                return 0;
            }
            return 0;
        }
    }
}