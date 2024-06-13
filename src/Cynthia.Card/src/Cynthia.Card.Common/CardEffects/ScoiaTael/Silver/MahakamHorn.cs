using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("53021")] //希姆
    public class MahakamHorn : Choosespell
    {
        //择一：从牌组打出1张铜色/银色“诅咒生物”牌；或创造对方初始牌组中1张银色单位牌。
        public MahakamHorn(GameCard card) : base(card)
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
                {1, "MahakamHorn_1_CreateDwarf"},
                {2, "MahakamHorn_2_Strenghten"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            return await Card.CreateAndMoveStay(
            GwentMap.GetCreateCardsId(
            x => x.HasAnyCategorie(Categorie.Dwarf) &&
            (x.Group == Group.Copper || x.Group == Group.Silver),
            RNG)
        .ToList());
        }

        private async Task<int> FUNCTION2()
        {
            var targets = await Game.GetSelectPlaceCards(Card, 1, selectMode: SelectModeType.MyRow);
            if (targets.Count() == 0)
            {
                return 0;
            }
            //强化7
            foreach (var target in targets)
            {
                await target.Effect.Strengthen(7, Card);
            }

            return 0;
        }
    }
}