using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64014")]//奎特家族捕鲸鱼叉手
    public class AnCraiteWhaler : CardEffect
    {//将1个敌军单位移至其所在半场的同排，并使它受到等同于所在排单位数量的伤害。
        public AnCraiteWhaler(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //计算对面排的单位数量
            var count = Game.RowToList(AnotherPlayer, Card.Status.CardRow).IgnoreConcealAndDead().Count();
            //如果对面单位数达到上限，不触发效果
            if (count >= Game.RowMaxCount)
            {
                return 0;
            }
            if (!(await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow, filter: x => x.Status.CardRow != Card.Status.CardRow)).TrySingle(out var target))
            {
                return 0;
            }

            await target.Effect.Move(new CardLocation(Card.Status.CardRow, int.MaxValue), Card);
            //又移动了一个单位 伤害为count+1
            await target.Effect.Damage(count + 1, Card);
            return 0;
        }
    }
}