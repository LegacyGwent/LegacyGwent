using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("22005")]//伊勒瑞斯
    public class Imlerith : CardEffect
    {//对1个敌军单位造成4点伤害，若目标位于“刺骨冰霜”之下，则将其摧毁。
        public Imlerith(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (!(await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow)).TrySingle(out var target))
            {
                return 0;
            }
            //int point = Game.GameRowEffect[AnotherPlayer][target.Status.CardRow.MyRowToIndex()].RowStatus == RowStatus.BitingFrost ? 8 : 4;
            if(Game.GameRowEffect[AnotherPlayer][target.Status.CardRow.MyRowToIndex()].RowStatus == RowStatus.BitingFrost)
            {
                await target.Effect.ToCemetery(CardBreakEffectType.Scorch);
            }
            else
            {
                await target.Effect.Damage(4, Card);
            }
            return 0;
        }
    }
}