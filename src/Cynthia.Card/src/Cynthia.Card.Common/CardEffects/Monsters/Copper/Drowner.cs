using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24014")]//水鬼
    public class Drowner : CardEffect
    {//将1个敌军单位拖至对方同排，对其造成2点伤害，若目标排处于灾厄之下，则伤害提高至4点。
        public Drowner(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //没有任何可以移动的情况
            if (Game.PlayersPlace[AnotherPlayer].Count() >= Game.RowMaxCount)
            {
                return 0;
            }
            if (!(await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow, filter: x => x.Status.CardRow != Card.Status.CardRow)).TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Move(new CardLocation(Card.Status.CardRow, int.MaxValue), Card);
            var damagePoint = Game.GameRowEffect[target.PlayerIndex][target.Status.CardRow.MyRowToIndex()].RowStatus.IsHazard() ? 5 : 2;
            await target.Effect.Damage(damagePoint, Card);
            return 0;
        }
    }
}