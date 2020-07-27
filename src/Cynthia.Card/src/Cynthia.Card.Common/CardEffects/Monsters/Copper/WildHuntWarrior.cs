using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24016")]//狂猎战士
    public class WildHuntWarrior : CardEffect
    {//对1个敌军单位造成4点伤害。若目标位于“刺骨冰霜”之下或被摧毁，则获得2点增益。
        public WildHuntWarrior(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            var isBoost = Game.GameRowEffect[target.PlayerIndex][target.Status.CardRow.MyRowToIndex()].RowStatus == RowStatus.BitingFrost;
            await target.Effect.Damage(4, Card);
            isBoost = isBoost || target.IsDead;
            if (isBoost)
            {
                await Boost(2, Card);
            }
            return 0;
        }
    }
}