using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70169")]//水生孽鬼
    public class NekkerSwarm : CardEffect
    {
        public NekkerSwarm(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var targets = await Game.GetSelectPlaceCards(
                Card,
                3,
                filter: target => target.PlayerIndex == PlayerIndex,
                selectMode: SelectModeType.MyRow);
            foreach (var target in targets)
            {
                if (target.HasAnyCategorie(Categorie.Ogroid))
                {
                    await target.Effect.Strengthen(1, Card);
                }
                else
                {
                    await target.Effect.Boost(1, Card);
                }
            }

            return 0;
        }
    }
}
