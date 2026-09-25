using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId(CardId.CatSchoolWitcherThug)]
    public class CatSchoolWitcherThug : CardEffect
    {
        public CatSchoolWitcherThug(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selected = await Game.GetSelectPlaceCards(
                Card, 2,
                filter: x => x.Status.CardRow != Card.Status.CardRow,
                selectMode: SelectModeType.EnemyRow);
            foreach (var target in selected)
            {
                await target.Effect.Move(new CardLocation(Card.Status.CardRow, int.MaxValue), Card);
            }
            for (var repeat = 0; repeat < 2; repeat++)
            {
                var enemyCount = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror())
                    .IgnoreConcealAndDead().Count;
                var damage = enemyCount - Card.Status.Strength;
                if (damage <= 0)
                {
                    break;
                }

                var damageTargets = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
                if (!damageTargets.TrySingle(out var damageTarget))
                {
                    break;
                }
                await damageTarget.Effect.Damage(damage, Card);
            }
            return 0;
        }
    }
}
