using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63013")]//尤娜
    public class Yoana : CardEffect
    {//治愈1个友军单位，随后使其获得等同于治疗量的增益。
        public Yoana(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //选取一个我方场上单位
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            if (target.Status.HealthStatus >= 0)
            {
                return 0;
            }
            await target.Effect.Heal(Card);
            await target.Effect.Boost(-target.Status.HealthStatus, Card);

            return 0;
        }
    }
}