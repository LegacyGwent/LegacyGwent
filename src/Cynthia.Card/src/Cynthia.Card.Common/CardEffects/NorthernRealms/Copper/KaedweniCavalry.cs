using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44004")]//科德温骑兵
    public class KaedweniCavalry : CardEffect
    {//摧毁1个单位的护甲。扣除的护甲值将被转化为自身增益。
        public KaedweniCavalry(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //还没有摧毁护甲api，目前用伤害
            //选择一张场上的卡
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow, filter: x => x.Status.Armor >= 0);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            int damagenum = target.Status.Armor;
            await target.Effect.Damage(damagenum, Card);
            await Card.Effect.Boost(damagenum, Card);
            return 0;
        }
    }
}