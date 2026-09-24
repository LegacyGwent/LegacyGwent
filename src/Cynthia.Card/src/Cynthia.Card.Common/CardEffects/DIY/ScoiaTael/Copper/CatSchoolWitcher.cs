using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId(CardId.CatSchoolWitcher)]
    public class CatSchoolWitcher : CardEffect
    {
        public CatSchoolWitcher(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var enemies = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror())
                .IgnoreConcealAndDead()
                .ToList();
            var damage = Card.Status.Strength - enemies.Count;
            if (damage > 0)
            {
                foreach (var enemy in enemies)
                {
                    await enemy.Effect.Damage(damage, Card);
                }
            }
            return 0;
        }
    }
}
