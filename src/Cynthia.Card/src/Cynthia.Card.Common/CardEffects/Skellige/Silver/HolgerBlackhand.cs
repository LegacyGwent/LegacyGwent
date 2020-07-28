using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63011")]//“黑手”霍格
    public class HolgerBlackhand : CardEffect
    {
        //造成7点伤害。若摧毁目标，则使己方墓场中最强的单位获得3点强化。
        public HolgerBlackhand(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {



            //以下代码基于：如果有多个最强单位，随机强化其中一个
            //选择一张场上的卡
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Damage(7, Card);
            //如果目标没死，结束
            if (!target.IsDead)
            {
                return 0;
            }

            var CemeteryList = Game.PlayersCemetery[Card.PlayerIndex].Where(x => x.CardInfo().CardType == CardType.Unit);
            if (CemeteryList.Count() == 0)
            {
                return 0;
            }

            //仿照WhereAllHighest,构造一个力量排序函数,和领袖奎特一致
            var StrengthList = CemeteryList.Select(x => (Strength: x.Status.Strength, card: x)).OrderByDescending(x => x.Strength);
            var StrengthMaximun = StrengthList.First().Strength;
            var result = StrengthList.Where(x => x.Strength >= StrengthMaximun).Select(x => x.card);

            if (!result.TryMessOne(out var StrengthenTarget, Game.RNG))
            {
                return 0;
            }
            await StrengthenTarget.Effect.Strengthen(3, Card);
            return 0;
        }
    }
}