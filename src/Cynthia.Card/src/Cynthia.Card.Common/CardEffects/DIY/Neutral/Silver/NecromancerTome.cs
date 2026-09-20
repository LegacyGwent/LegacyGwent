using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70193")]// necromancer tome
    public class NecromancerTome : CardEffect
    {//Destroy up to 4 friendly units. Spawn a Specter in their place.
    // 摧毁4个友方单位，并在原位各生成1个"鬼灵"。

        public NecromancerTome(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var targets = await Game.GetSelectPlaceCards(Card, 4, selectMode: SelectModeType.MyRow, isHasConceal: true);
            if (targets.Count() == 0)
            {
                return 0;
            }
            foreach (var target in targets)
            {
                var position = target.GetLocation();
                await target.Effect.Boost(target.CardPoint(), Card);
                await target.Effect.ToCemetery(CardBreakEffectType.Epidemic);
                await Game.CreateCard(CardId.Specter, PlayerIndex, position);
            }
            return 0;
        }
    }
}