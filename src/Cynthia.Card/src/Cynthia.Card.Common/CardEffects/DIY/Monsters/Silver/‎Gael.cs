using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;


namespace Cynthia.Card
{
    [CardEffectId("70146")]//加尔 Gael "Deploy, Choose One: Deploy: Deal 5 Damage to two enemy units each or Damage an enemy by 8."
    public class Gael : Choose
    {//
        public Gael(GameCard card) : base(card) { }
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
                {1, "Gael_1_Damage2Units"},
                {2, "Gael_1_Damage1Unit"}
            };
        }

        // Deal 5 Damage to two enemy units each
        private async Task<int> FUNCTION1()
        { 
            var targets = await Game.GetSelectPlaceCards(Card, 2, selectMode: SelectModeType.EnemyRow);
            if (targets.Count() == 0)
            {
                return 0;
            }
            foreach (var target in targets)
            {
                await target.Effect.Damage(5, Card);
            }

            return 0;
        }
            // Damage an enemy by 8.
        private async Task<int> FUNCTION2()
        {
            var targets = await Game.GetSelectPlaceCards(Card, 1, selectMode: SelectModeType.EnemyRow);
            if (targets.Count() == 0)
            {
                return 0;
            }
            foreach (var target in targets)
            {
                await target.Effect.Damage(8, Card);
            }

            return 0;
        }
    }
}

