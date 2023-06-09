using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64019")]//迪门家族战船
    public class DimunWarship : CardEffect
    {//连续4次对同一个单位造成1点伤害。
        public DimunWarship(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //选择一张场上的卡
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            //伤害四次
            for (var i = 0; i < 5; i++)
            {
                await target.Effect.Damage(1, Card, BulletType.FireBall);
            }
            return 0;
        }
    }
}