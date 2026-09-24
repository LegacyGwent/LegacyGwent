using System;
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
            var allies = Game.RowToList(PlayerIndex, Card.Status.CardRow)
                .IgnoreConcealAndDead()
                .Where(x => x != Card)
                .ToList();
            var damageDealt = 0;
            foreach (var ally in allies)
            {
                var before = Math.Max(0, ally.CardPoint()) + ally.Status.Armor;
                await ally.Effect.Damage(1, Card);
                var after = ally.IsDead ? 0 : Math.Max(0, ally.CardPoint()) + ally.Status.Armor;
                damageDealt += Math.Max(0, before - after);
            }
            await Card.Effect.Strengthen(damageDealt / 2, Card);

            var enemies = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror())
                .IgnoreConcealAndDead()
                .ToList();
            var difference = Card.Status.Strength - enemies.Count;
            if (difference > 0)
            {
                foreach (var enemy in enemies)
                {
                    await enemy.Effect.Damage(difference, Card);
                }
            }
            return 0;
        }
    }
}
