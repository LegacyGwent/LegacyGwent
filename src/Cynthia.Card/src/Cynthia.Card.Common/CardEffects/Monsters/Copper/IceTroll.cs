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
            //如果目标在冰霜下仿照Duel 写一个类似对决
            if (Game.GameRowEffect[AnotherPlayer][target.Status.CardRow.MyRowToIndex()].RowStatus == RowStatus.BitingFrost)
            {
                int count = 0;
                while (true)
                {
                    count++;
                    await target.Effect.Damage(2 * Card.CardPoint(), Card, BulletType.RedLight);
                    if (target.IsDead || !target.Status.CardRow.IsOnPlace())
                    {
                        break;
                    }
                    await Game.ClientDelay(400);
                    await Card.Effect.Damage(target.CardPoint(), target, BulletType.RedLight);
                    if (Card.IsDead || !Card.Status.CardRow.IsOnPlace())
                    {
                        break;
                    }
                    await Game.ClientDelay(400);
                    if (count > 20)
                    {
                        break;
                    }
                }
                return 0;
            }
            //正常对决，target先受到伤害
            await Duel(target, Card);
            return 0;
        }
    }
}