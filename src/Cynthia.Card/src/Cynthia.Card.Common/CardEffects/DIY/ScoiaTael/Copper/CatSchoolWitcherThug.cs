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
                Card,
                filter: x => x.Status.CardRow != Card.Status.CardRow,
                selectMode: SelectModeType.EnemyRow);
            if (!selected.TrySingle(out var target))
            {
                return 0;
            }

            await target.Effect.Move(new CardLocation(Card.Status.CardRow, int.MaxValue), Card);
            for (var repeat = 0; repeat < 2; repeat++)
            {
                var enemyCount = Game.RowToList(PlayerIndex, Card.Status.CardRow.Mirror())
                    .IgnoreConcealAndDead().Count;
                var damage = enemyCount - Card.Status.Strength;
                if (damage <= 0 || target.IsDead || !target.Status.CardRow.IsOnPlace())
                {
                    break;
                }
                await target.Effect.Damage(damage, Card);
                if (!target.IsDead && target.Status.CardRow.IsOnPlace())
                {
                    await target.Effect.Damage(damage, Card);
                }
            }
            return 0;
        }
    }
}
