using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24008")]//狮鹫
    public class Griffin : CardEffect
    {//触发1个铜色友军单位的遗愿效果。
        public Griffin(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow, filter: x => x.Status.Group == Group.Copper);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            //不严格的写法 可能会触发除了选中卡遗愿以外的效果
            if (target.Status.IsLock)
            {
                return 0;
            }
            await target.Effects.RaiseEvent(new AfterCardDeath(target, target.GetLocation()));
            return 0;
        }
    }
}