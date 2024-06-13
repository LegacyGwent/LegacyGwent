using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("13040")] //希姆
    public class Mandrake : Choosespell
    {
        //择一：从牌组打出1张铜色/银色“诅咒生物”牌；或创造对方初始牌组中1张银色单位牌。
        public Mandrake(GameCard card) : base(card)
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
                {1, "Mandrake_1_Strenghten"},
                {2, "Mandrake_2_Weaken"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            var target = await Game.GetSelectPlaceCards(Card);
            if (target.Count <= 0) return 0;
            var tagetCard = target.Single();
            await tagetCard.Effect.Heal(Card);
            await tagetCard.Effect.Strengthen(6, Card);
            return 0;
        }

        private async Task<int> FUNCTION2()
        {
            var target = await Game.GetSelectPlaceCards(Card);
            if (target.Count <= 0) return 0;
            var tagetCard = target.Single();
            await tagetCard.Effect.Reset(Card);
            await tagetCard.Effect.Weaken(6, Card);
            return 0;
        }
    }
}