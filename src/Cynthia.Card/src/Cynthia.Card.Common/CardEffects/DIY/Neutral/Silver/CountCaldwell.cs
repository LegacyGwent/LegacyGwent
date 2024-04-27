using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70131")]//考德威尔伯爵 CountCaldwell
    public class CountCaldwell : CardEffect
    {//交换2个敌军单位的基础战力
        public CountCaldwell(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var select1 = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!select1.TrySingle(out var target1))
            {
                return 0;
            }
            var select2 = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!select2.TrySingle(out var target2))
            {
                return 0;
            }

            int offset = select1.Single().Status.Strength - select2.Single().Status.Strength;
            if (select1.Single().Status.Strength > select2.Single().Status.Strength)
            {
                await select1.Single().Effect.Weaken(offset, Card);
                await select2.Single().Effect.Strengthen(offset, Card);
            }
            if (select1.Single().Status.Strength < select2.Single().Status.Strength)
            {
                await select2.Single().Effect.Weaken(-offset, Card);
                await select1.Single().Effect.Strengthen(-offset, Card);
            }
            return 0;
        }
    }
}
