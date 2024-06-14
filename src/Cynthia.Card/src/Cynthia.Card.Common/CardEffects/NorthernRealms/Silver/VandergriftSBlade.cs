using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("43021")]//伊森格林：亡命徒
    public class VandergriftSBlade : Choosespell
    {//择一：从牌组打出1张铜色/银色“特殊”牌；或创造1个银色“精灵”单位。
        public VandergriftSBlade(GameCard card) : base(card)
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
                {1, "VandergriftSBlade_1_DestroyCursed"},
                {2, "VandergriftSBlade_2_Damage"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            var target = await Game.GetSelectPlaceCards(Card, 1, false,
                x => x.Status.Categories.Contains(Categorie.Cursed) && (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver));
            if (target.Count == 0)
            {
                return 0;
            }
            await target.Single().Effect.ToCemetery(CardBreakEffectType.Scorch);
            return 0;            
        }

        private async Task<int> FUNCTION2()
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Damage(10, Card);
            //如果目标没死，结束
            if (!target.IsDead)
            {
                return 0;
            }
            target.Status.IsDoomed = true;
            return 0;
        }
    }
}
