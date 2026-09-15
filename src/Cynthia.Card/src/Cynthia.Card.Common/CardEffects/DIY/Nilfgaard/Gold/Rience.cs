using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70201")]//里恩斯 Rience
    public class Rience : CardEffect
    {
        public Rience(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(
                Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }

            var power = target.CardPoint();
            var adjacent = target.GetRangeCard(1, GetRangeType.HollowAll).ToList();
            await target.Effect.ToCemetery(CardBreakEffectType.Scorch);

            foreach (var unit in adjacent.Where(x => x.Status.CardRow.IsOnPlace() && !x.Status.Conceal))
            {
                await unit.Effect.Boost((power + 1) / 2, Card);
            }
            return 0;
        }
    }
}
