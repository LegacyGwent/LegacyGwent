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
            //先记录
            damagenum = result.First().Status.Strength;
            //后丢弃
            await result.First().Effect.Discard(Card);

            //选择一张场上的卡
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Damage(damagenum, Card);


            return 0;
        }
    }
}