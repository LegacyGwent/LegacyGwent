using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70085")]//尼斯里拉
    public class TheApiarianPhantom : CardEffect
    {//对1个敌军单位造成6点伤害。手牌中每有1张“狂猎”单位牌，伤害提高1点。
        public TheApiarianPhantom(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {

            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            var targetRow = Game.GameRowEffect[target.PlayerIndex][target.Status.CardRow.MyRowToIndex()];
            await target.Effect.Damage(6, Card);
            // Nested game tasks may not have moved a lethally damaged unit to the cemetery yet.
            // Accept either observable state so Frost is never lost to queue timing.
            if (!target.Status.CardRow.IsOnPlace() || target.CardPoint() <= 0)
            {
                await targetRow.SetStatus<BitingFrostStatus>();
            }
            return 0;

        }
    }
}
