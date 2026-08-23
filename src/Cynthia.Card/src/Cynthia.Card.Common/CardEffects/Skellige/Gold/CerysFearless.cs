using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62010")]//凯瑞丝：无所畏惧
    public class CerysFearless : CardEffect
    {//与1个敌军单位对决，若存活，使1名友方“德拉蒙女王卫队”与1个敌军单位对决。
        public CerysFearless(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (!(await Game.GetSelectPlaceCards(
                Card,
                selectMode: SelectModeType.EnemyRow)).TrySingle(out var firstEnemy))
            {
                return 0;
            }

            await Duel(firstEnemy, Card);
            if (!Card.IsAliveOnPlance())
            {
                return 0;
            }

            if (!(await Game.GetSelectPlaceCards(
                Card,
                selectMode: SelectModeType.MyRow,
                filter: card => card.Status.CardId == CardId.DrummondQueensguard))
                .TrySingle(out var queensguard))
            {
                return 0;
            }

            if (!(await Game.GetSelectPlaceCards(
                Card,
                selectMode: SelectModeType.EnemyRow)).TrySingle(out var secondEnemy))
            {
                return 0;
            }

            await queensguard.Effect.Duel(secondEnemy, Card);
            return 0;
        }
    }
}
