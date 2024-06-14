using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("64034")]//骨制护符
    public class BoneTalisman : Choosespell
    {//择一：复活1个铜色“野兽”或“呓语”单位；或治愈1名友军单位，并使其获得3点强化。
        public BoneTalisman(GameCard card) : base(card)
        {
        }

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
                {1, "BoneTalisman_1_ResurectBeast"},
                {2, "BoneTalisman_2_Strenghten"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            var list = Game.PlayersCemetery[PlayerIndex].Where(x => x.Status.Group == Group.Copper && x.CardInfo().CardType == CardType.Unit && x.HasAnyCategorie(Categorie.Beast, Categorie.Cultist)).Mess(Game.RNG);
            if (list.Count() == 0)
            {
                return 0;
            }
            var result = await Game.GetSelectMenuCards
            (Card.PlayerIndex, list.ToList(), 1, "选择复活一张牌");
            if (result.Count() == 0)
            {
                return 0;
            }
            await result.First().Effect.Resurrect(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
            return 1;
        }

        private async Task<int> FUNCTION2()
        {
            var targets = await Game.GetSelectPlaceCards(Card, 1, selectMode: SelectModeType.MyRow);
            if (targets.Count() == 0)
            {
                return 0;
            }
            foreach (var target in targets)
            {
                await target.Effect.Heal(Card);
                await target.Effect.Strengthen(3, Card);
            }
            return 0;
        }
    }
}
