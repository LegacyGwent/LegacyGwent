using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62008")]//“疯子”卢戈
    public class MadmanLugos : CardEffect
    {//从牌组丢弃1张铜色单位牌，对1个敌军单位造成等同于被丢弃单位基础战力的伤害。
        public MadmanLugos(GameCard card) : base(card) { }
        private int damagenum = 0;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //以下代码基于：先伤害 再丢弃 比较方便结算
            //乱序列出牌库中铜色单位
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Group == Group.Copper).Mess(Game.RNG);
            if (list.Count() == 0)
            {
                return 0;
            }
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 1, "选择丢弃一张牌");

            if (result.Count() == 0)
            {
                return 0;
            }

            //选择一张场上的卡
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }

            await target.Effect.Damage(result.First().Status.Strength, Card);

            await result.First().Effect.Discard(Card);



            return 0;
        }
    }
}