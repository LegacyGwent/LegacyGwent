using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("240140")]//水鬼：晋升
    public class DrownerPro : CardEffect
    {//将2个敌军单位拖至对方同排，对其造成2点伤害，若目标排处于灾厄之下，则伤害提高至4点。
        public DrownerPro(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //没有任何可以移动的情况
            for (var i = 0; i < 2; i++)
            {
                if (Game.PlayersPlace[AnotherPlayer].Count() >= Game.RowMaxCount)
                {
                    return 0;
                }
                if (!(await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow, filter: x => x.Status.CardRow != Card.Status.CardRow)).TrySingle(out var target))
                {
                    return 0;
                }
                await target.Effect.Move(new CardLocation(Card.Status.CardRow, int.MaxValue), Card);
                var damagePoint = Game.GameRowEffect[target.PlayerIndex][target.Status.CardRow.MyRowToIndex()].RowStatus.IsHazard() ? 4 : 2;
                await target.Effect.Damage(damagePoint, Card);
            }
            return 0;
        }
    }
}