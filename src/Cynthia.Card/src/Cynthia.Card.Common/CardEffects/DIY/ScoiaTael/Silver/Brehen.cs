using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId(CardId.Brehen)]
    public class Brehen : CardEffect
    {
        public Brehen(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var rowUnitCount = Game.RowToList(PlayerIndex, Card.Status.CardRow)
                .Count(x => x != Card && !x.IsDead);
            var allies = Game.RowToList(PlayerIndex, Card.Status.CardRow)
                .IgnoreConcealAndDead()
                .Where(x => x != Card)
                .ToList();
            foreach (var ally in allies)
            {
                await ally.Effect.Damage(1, Card);
            }

            var enemies = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror())
                .IgnoreConcealAndDead()
                .ToList();
            var enemyCount = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror()).Count(x => !x.IsDead);
            var difference = Card.Status.Strength - enemyCount;
            if (difference > 0)
            {
                foreach (var enemy in enemies)
                {
                    await enemy.Effect.Damage(difference + rowUnitCount / 2, Card);
                }
            }
            return 0;
        }
    }
}
