using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70100")]//林语者 ForestWhisperer
    public class ForestWhisperer : CardEffect
    {//对双方同排的非树精单位造成2点伤害。
        public ForestWhisperer(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var cards1 = Game.RowToList(AnotherPlayer, Card.Status.CardRow).IgnoreConcealAndDead().Where(x => x.Status.CardRow.IsOnPlace() && !x.HasAllCategorie(Categorie.Dryad) && x != Card).ToList();
            var cards2 = Game.RowToList(PlayerIndex, Card.Status.CardRow).IgnoreConcealAndDead().Where(x => x.Status.CardRow.IsOnPlace() && !x.HasAllCategorie(Categorie.Dryad) && x != Card).ToList();
            
            foreach (var card in cards1)
            {
                await card.Effect.Damage(2, Card, BulletType.RedLight);
            }
            foreach (var card in cards2)
            {
                await card.Effect.Damage(2, Card, BulletType.RedLight);
            }
            return 0;
        }
    }
}
