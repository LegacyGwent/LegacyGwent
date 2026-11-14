using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;


namespace Cynthia.Card
{
    [CardEffectId("70122")]//树精族母 DryadMatron
    public class DryadMatron : CardEffect
    {// Strengthen the row first, then boost one selected hand unit by the total.
        public DryadMatron(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var dryads = Game.RowToList(PlayerIndex, Card.Status.CardRow)
                .IgnoreConcealAndDead()
                .Where(x => x != Card && x.HasAnyCategorie(Categorie.Dryad))
                .ToList();

            var totalStrengthening = 0;
            foreach (var dryad in dryads)
            {
                var before = dryad.Status.Strength;
                await dryad.Effect.Strengthen(1, Card);
                totalStrengthening += System.Math.Max(0, dryad.Status.Strength - before);
            }
            var handTargets = Game.PlayersHandCard[PlayerIndex]
                .Where(x => x.CardInfo().CardType == CardType.Unit).ToList();
            if (totalStrengthening > 0 && handTargets.Count > 0)
            {
                var selected = await Game.GetSelectMenuCards(PlayerIndex, handTargets, 1, "选择增益1张单位牌");
                if (selected.Count > 0)
                    await selected.First().Effect.Boost(totalStrengthening, Card);
            }
            return 0;
        }
        
       
    }
}
