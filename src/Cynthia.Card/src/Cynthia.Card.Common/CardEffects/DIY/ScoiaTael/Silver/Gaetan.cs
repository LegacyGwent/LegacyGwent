using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId(CardId.Gaetan)]
    public class Gaetan : CardEffect
    {
        public Gaetan(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var allies = Game.RowToList(PlayerIndex, Card.Status.CardRow)
                .IgnoreConcealAndDead()
                .Where(x => x != Card)
                .ToList();
            foreach (var ally in allies)
            {
                await ally.Effect.Damage(1, Card);
            }

            var repetitions = 1 + Game.RowToList(PlayerIndex, Card.Status.CardRow)
                .IgnoreConcealAndDead()
                .Count(x => x != Card);
            for (var index = 0; index < repetitions; index++)
            {
                var enemyCount = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror())
                    .IgnoreConcealAndDead().Count;
                var damage = enemyCount - Card.Status.Strength;
                if (damage <= 0)
                {
                    break;
                }

                var selected = await Game.GetSelectPlaceCards(
                    Card,
                    filter: x => x.Status.CardRow == Card.Status.CardRow,
                    selectMode: SelectModeType.EnemyRow);
                if (!selected.TrySingle(out var target))
                {
                    break;
                }
                await target.Effect.Damage(damage, Card);
            }
            return 0;
        }
    }
}
