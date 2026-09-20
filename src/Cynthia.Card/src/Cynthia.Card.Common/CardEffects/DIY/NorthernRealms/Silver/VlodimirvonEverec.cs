using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("70192")]//弗洛迪米·伊佛瑞克
    public class VlodimirvonEverec : Choose
    {//Choose one: Spawn a Specter to the left and right of this unit; or Destroy 2 friendly units. Boost self by their power and Spawn a Specter in their place;
    // 择一：在左右两侧生成1个鬼灵；或摧毁2个友军单位，获得与其战力相等的增益，并在原位各生成1个鬼灵。
        public VlodimirvonEverec(GameCard card) : base(card) { }
        protected override async Task<int> UseMethodByChoice(int switchCard)
        {
            switch (switchCard)
            {
                case 1:
                    return await FUNCTION1();
                case 2:
                    return await FUNCTION2();
            }

            return 0;
        }
        protected override void RealInitDict()
        {
            methodDesDict = new Dictionary<int, string>()
            {
                {1, "Vlodimir_1_Spawn2"},
                {2, "Vlodimir_2_Destroy2"}
            };
        }
        private async Task<int> FUNCTION1()
        {
            if (!Card.Status.CardRow.IsOnPlace()) return 0;
            var position = Card.GetLocation();
            await Game.CreateCard(CardId.Specter, PlayerIndex, Card.GetLocation());
            await Game.CreateCard(CardId.Specter, PlayerIndex, Card.GetLocation().With(x => x.CardIndex++));
            return 0;
        }
        private async Task<int> FUNCTION2()
        {
            if (!Card.Status.CardRow.IsOnPlace()) return 0;

            var targets = await Game.GetSelectPlaceCards(Card, 2, selectMode: SelectModeType.MyRow, isHasConceal: true);
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
