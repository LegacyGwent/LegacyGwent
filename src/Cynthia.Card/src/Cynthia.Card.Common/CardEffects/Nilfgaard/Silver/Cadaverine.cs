using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("33021")]//吊死鬼之毒
    public class Cadaverine : Choosespell
    {//择一：对1个敌军单位以及所有与它同类型的单位造成3点伤害；或摧毁1个铜色/银色“中立”单位。
        public Cadaverine(GameCard card) : base(card)
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
                {1, "Cadaverine_1_DamegeCategory"},
                {2, "Cadaverine_2_DestroyNeutral"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            var cards = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (cards.Count == 0)
            {
                return 0;
            }
            var categories = cards.Single().Status.Categories;
            await Game.Debug("标签开始筛选:" + categories.Join(","));
            var targetCards = Game.GetAllCard(PlayerIndex).Where(x => x.PlayerIndex == AnotherPlayer && x.Status.CardRow.IsOnPlace() && x.Status.Categories.Intersect(categories).Any());
            await Game.Debug($"筛选出了{targetCards.Count()}个");
            foreach (var card in targetCards)
            {
                await card.Effect.Damage(3, Card, BulletType.RedLight);
            }
            return 0;
        }

        private async Task<int> FUNCTION2()
        {
            var cards = await Game.GetSelectPlaceCards(Card, filter:
                (x => (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver) && x.Status.Faction == Faction.Neutral)
                , selectMode: SelectModeType.EnemyRow);
            if (cards.Count == 0)
            {
                return 0;
            }
            await cards.Single().Effect.ToCemetery(CardBreakEffectType.Scorch);
            return 0;
        }
    }
}
