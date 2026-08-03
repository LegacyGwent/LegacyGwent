using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24034")]//冰巨魔
    public class IceTroll : CardEffect
    {//与1个敌军单位对决。若它位于“刺骨冰霜”之下，则己方伤害翻倍。
        public IceTroll(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {

            //选一张牌，必须选
            var list = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            //如果没有，什么都不发生
            if (!list.TrySingle(out var target))
            {
                return 0;
            }
            var damageMultiplier = Game.GameRowEffect[AnotherPlayer]
                [target.Status.CardRow.MyRowToIndex()].RowStatus == RowStatus.BitingFrost
                ? 2
                : 1;
            await Duel(target, Card, damageMultiplier);
            return 0;
        }
    }
}
