using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("44033")]//绞盘
    public class Winch : Choosespell
    {//使所有己方半场的“机械”单位获得3点增益。 从墓场复活1个铜色“机械”单位。
        public Winch(GameCard card) : base(card)
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
                {1, "Winch_1_Resurect"},
                {2, "Winch_2_Boost"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            var list = Game.PlayersCemetery[PlayerIndex].Where(x => (x.Status.Group == Group.Copper) && x.CardInfo().CardType == CardType.Unit && x.HasAnyCategorie(Categorie.Machine)).ToList();
            if (list.Count() == 0)
            {
                return 0;
            }
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
            (Card.PlayerIndex, list.ToList(), 1, "选择复活一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (result.Count() == 0)
            {
                return 0;
            }
            var resurrectCard = result.First();
            resurrectCard.Status.IsDoomed = true;
            await resurrectCard.Effect.Resurrect(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
            return 1;
        }

        private async Task<int> FUNCTION2()
        {
            var list = Game.GetPlaceCards(PlayerIndex).FilterCards(filter: x => x.HasAllCategorie(Categorie.Machine) && x.CardInfo().CardType == CardType.Unit);
            foreach (var card in list)
            {
                await card.Effect.Boost(3, Card);
            }
            return 0;
        }
    }
}
