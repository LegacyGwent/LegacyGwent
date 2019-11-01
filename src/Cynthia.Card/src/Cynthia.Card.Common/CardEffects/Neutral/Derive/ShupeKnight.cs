using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("15001")] //店店：骑士
    public class ShupeKnight : AbstractShupe
    {
        //派“店店”去帝国宫廷军事学院。 强化自身至25点；坚韧；与1个敌军单位对决；重置1个单位；摧毁所有战力低于4点的敌军单位。
        public ShupeKnight(GameCard card) : base(card)
        {
        }

        protected override async Task<int> UseMethodByChoice(int switchCard)
        {
            switch (switchCard)
            {
                case 1:
                    return await StrengthenMyself();
                case 2:
                    return await ResilienceMySelf();
                case 3:
                    return await DuelEnemy();
                case 4:
                    return await ResetUnit();
                case 5:
                    return await DestroySmallUnit();
            }

            return 0;
        }

        private async Task<int> DestroySmallUnit()
        {
            var list = Game.GetPlaceCards(AnotherPlayer).Where(x => x.CardPoint() < 4).ToList();

            foreach (var it in list)
            {
                await it.Effect.ToCemetery();
            }

            return 0;
        }

        private async Task<int> ResetUnit()
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }

            await target.Effect.Reset(Card);
            return 0;
        }

        private async Task<int> DuelEnemy()
        {
            //选一张牌，必须选
            var list = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            //如果没有，什么都不发生
            if (!list.TrySingle(out var target))
            {
                return 0;
            }

            //对决，target先受到伤害
            await Duel(target, Card);
            return 0;
        }

        private async Task<int> ResilienceMySelf()
        {
            await Card.Effect.Resilience(Card);
            return 0;
        }

        private async Task<int> StrengthenMyself()
        {
            int offset = 25 - Card.CardPoint();
            await Card.Effect.Strengthen(offset, Card);
            return 0;
        }

        protected override void RealInitDict()
        {
            methodDesDict = new Dictionary<int, string>()
            {
                {1, "强化自身至25点"},
                {2, "坚韧"},
                {3, "与1个敌军单位对决"},
                {4, "重置1个单位"},
                {5, "摧毁所有战力低于4点的敌军单位"}
            };
        }
    }
}